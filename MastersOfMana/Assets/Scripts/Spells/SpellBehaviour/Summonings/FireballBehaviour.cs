using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The specific behaviour of the fireball, that is manifested in the scene
/// </summary>
public class FireballBehaviour : A_ServerMoveableSummoning
{
    public float speed = 50.0f;
    public int directDamage = 8;

    public Color effectColor;
	//public GameObject ballPrefab;
	public GameObject explosionPrefab;
	public float explosionRadius = 4;
	public float explosionForce = 5;
	public int explosionDamage = 4;

	public float disappearTimer = 3;

    public GameObject[] DeactivatedObjectsOnCollision;

	public PreviewSpell preview;

    public override void Awake()
    {
        base.Awake();

        RFX4_ColorHelper.ChangeObjectColorByHUE(gameObject, RFX4_ColorHelper.ColorToHSV(effectColor).H);

        if (!mRigid)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException();
        }
    }

	public override void Preview (PlayerScript caster)
	{
		base.Preview(caster);

//		if(!sPreview)
//		{
//			sPreview = GameObject.Instantiate(previewPrefab) as PreviewSpell;
//		}

		RaycastHit hit;
		Vector3 aimDirection = GetAimLocal(caster, out hit);
	
		if(Physics.SphereCast(caster.handTransform.position, 0.25f, aimDirection, out hit))
		{
			preview.instance.MoveAndRotate(hit.point + hit.normal * 0.25f, Quaternion.LookRotation(hit.normal));
		}
		else
		{
			preview.instance.Deactivate();
		}
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
		preview.instance.Deactivate();
	}

    public override void Execute(PlayerScript caster)
    {
        //Get a fireballinstance out of the pool
		//FireballBehaviour fireballBehaviour = PoolRegistry.FireballPool.Get().GetComponent<FireballBehaviour>();
		FireballBehaviour fireballBehaviour = PoolRegistry.Instantiate(this.gameObject).GetComponent<FireballBehaviour>();

        //now activate it, so no weird interpolation errors occcure
        //TODO delete this eventually - RPCs are just too slow
        //fireball.GetComponent<A_SummoningBehaviour>().RpcSetActive(true);
        fireballBehaviour.gameObject.SetActive(true);

		Vector3 aimDirection = GetAim(caster); 

		//position the fireball to 'spawn' at the casters hand
		fireballBehaviour.Reset(caster.handTransform.position, caster.transform.rotation);
		//speed up the fireball to fly into the lookdirection of the player
		fireballBehaviour.mRigid.velocity = aimDirection * speed;
        fireballBehaviour.caster = caster;

        OnCollisionDeactivateBehaviour(true);

        //create an instance of this fireball on the client's machine
        NetworkServer.Spawn(fireballBehaviour.gameObject, fireballBehaviour.GetComponent<NetworkIdentity>().assetId);

		fireballBehaviour.Disappear();
        //fireballBehaviour.StartCoroutine(Done()); <- this doesn't work, not sure why...
    }

	void Reset (Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;

		mRigid.Reset();
		mRigid.isKinematic = false;

        cachedHealth = new List<HealthScript>();
	}

    private List<HealthScript> cachedHealth;

    protected override void ExecuteTriggerEnter_Host(Collider collider)
    {
        if (collider.isTrigger || caster.IsColliderPartOf(collider))
        {
            return;
        }

		Vector3 directHitForce = mRigid.velocity;
		mRigid.isKinematic = true;

        if (!isServer)
		{
			return;
        }
		
        RpcExplosion(transform.position, transform.rotation);

        HealthScript directHit = null;

        if(!collider.gameObject.isStatic)
        {
            directHit = collider.gameObject.GetComponentInParent<HealthScript>();
            if (directHit && !cachedHealth.Contains(directHit))
            {
                cachedHealth.Add(directHit);

                Debug.Log("direct damage: " + directDamage);
                directHit.TakeDamage(directDamage, GetType());

                PlayerScript player = directHit.GetComponent<PlayerScript>();
                if (player)
                {
                    directHitForce.Normalize();
                    directHitForce *= explosionForce;
                    player.movement.RpcAddForce(directHitForce, ForceMode.VelocityChange);
                }
            }
        }

        Collider[] colliders = Physics.OverlapSphere(mRigid.position,explosionRadius);

        //TODO: TEMP SOLUTION
        List<Rigidbody> cachedRigidbodies = new List<Rigidbody>(colliders.Length);

        foreach(Collider c in colliders)
        {

            HealthScript health = c.GetComponentInParent<HealthScript>();

            if(health && health == directHit)
            {
                //took already damage and got a force
                continue;
            }

            if(health && !cachedHealth.Contains(health))
            {
                health.TakeDamage(explosionDamage, GetType());
                cachedHealth.Add(health);
            }
                
            if(c.attachedRigidbody && !cachedRigidbodies.Contains(c.attachedRigidbody))
            {
                cachedRigidbodies.Add(c.attachedRigidbody);

                Vector3 force = c.attachedRigidbody.worldCenterOfMass - mRigid.position; 

                force.Normalize();
                force *= explosionForce;

                //check wheather or not we are handling a player or just some random rigidbody
                if (c.attachedRigidbody.CompareTag("Player"))
                {
                    PlayerScript ps = c.attachedRigidbody.GetComponent<PlayerScript>();
                    ps.movement.RpcAddForce(force, ForceMode.VelocityChange);
                }
                else
                {
                    c.attachedRigidbody.AddForce(force, ForceMode.VelocityChange);
                }
            }
        }

        //var explosionObject = Instantiate(explosionPrefab, transform.position, transform.rotation);
        //RFX4_ColorHelper.ChangeObjectColorByHUE(explosionObject, RFX4_ColorHelper.ColorToHSV(effectColor).H);
        //OnCollisionDeactivateBehaviour(false);
    }

    void OnCollisionDeactivateBehaviour(bool active)
    {
        foreach (var effect in DeactivatedObjectsOnCollision)
        {
            //Debug.Log((active ? "" : "de") + "activate " + effect.name);
            effect.SetActive(active);
        }
    }

    [ClientRpc]
	void RpcExplosion(Vector3 position, Quaternion rotation)
	{
        mRigid.isKinematic = true;
        var explosionObject = Instantiate(explosionPrefab, position, rotation);
        RFX4_ColorHelper.ChangeObjectColorByHUE(explosionObject, RFX4_ColorHelper.ColorToHSV(effectColor).H);
        OnCollisionDeactivateBehaviour(false);
    }

    public void Disappear()
    {
        StartCoroutine(Done());
    }

    public IEnumerator Done()
	{
		yield return new WaitForSeconds(disappearTimer);

        OnCollisionDeactivateBehaviour(true);
        gameObject.SetActive(false);
		NetworkServer.UnSpawn(gameObject);
	}


	void OnValidate()
	{
		if(explosionPrefab != null)
		{
			explosionPrefab.transform.localScale = Vector3.one * explosionRadius * 2;
		}
	}
}