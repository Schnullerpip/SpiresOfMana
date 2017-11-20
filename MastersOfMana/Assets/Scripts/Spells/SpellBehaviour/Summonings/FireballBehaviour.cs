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
    [SerializeField]
    private float mSpeed = 5.0f;
    [SerializeField]
    private float mDamage = 5.0f;

    public Color effectColor;
	//public GameObject ballPrefab;
	public GameObject explosionPrefab;
	public float explosionRadius = 4;
	public float explosionForce = 5;
	public float explosionDamage = 5.0f;

	public float disappearTimer = 3;

    public GameObject[] DeactivatedObjectsOnCollision;

	public PreviewSpell previewPrefab;
	private static PreviewSpell sPreview; 

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

		if(!sPreview)
		{
			sPreview = GameObject.Instantiate(previewPrefab) as PreviewSpell;
		}

		RaycastHit hit;
		Vector3 aimDirection = GetAimLocal(caster, out hit);
	
		if(Physics.SphereCast(caster.handTransform.position, 0.25f, aimDirection, out hit))
		{
			sPreview.MoveAndRotate(hit.point + hit.normal * 0.25f, Quaternion.LookRotation(hit.normal));
		}
		else
		{
			sPreview.Deactivate();
		}
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
		if(sPreview)
		{
			sPreview.Deactivate();
		}
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

		//position the fireball to 'spawn' at the casters hand, including an offset so it does not collide instantly with the hand
		fireballBehaviour.Reset(caster.handTransform.position + aimDirection, caster.transform.rotation);
		//speed up the fireball to fly into the lookdirection of the player
		fireballBehaviour.mRigid.velocity = aimDirection * mSpeed;

        OnCollisionDeactivateBehaviour(true);

        //create an instance of this fireball on the client's machine
        NetworkServer.Spawn(fireballBehaviour.gameObject, fireballBehaviour.GetComponent<NetworkIdentity>().assetId);

		fireballBehaviour.Disappear();
    }

	void Reset (Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;

		mRigid.Reset();
		mRigid.isKinematic = false;
	}

    protected override void ExecuteTriggerEnter_Host(Collider collider)
    {
        if (collider.isTrigger)
        {
            return;
        }

		Vector3 directHitForce = mRigid.velocity;
		mRigid.isKinematic = true;

        if (!isServer)
		{
			return;
		}

		HealthScript directHit = collider.gameObject.GetComponentInParent<HealthScript>();
        if (directHit)
        {
            directHit.TakeDamage(mDamage);

            PlayerScript player = directHit.GetComponent<PlayerScript>();
            if (player)
            {
                directHitForce.Normalize();
                directHitForce *= explosionForce;
                player.movement.RpcAddForce(directHitForce, ForceMode.VelocityChange);
            }
        }

		Collider[] colliders = Physics.OverlapSphere(mRigid.position,explosionRadius);

		//TODO: TEMP SOLUTION
		List<HealthScript> cachedHealthScripts = new List<HealthScript>(colliders.Length);
		List<Rigidbody> cachedRigidbodies = new List<Rigidbody>(colliders.Length);

		foreach(Collider c in colliders)
		{

			HealthScript health = c.GetComponentInParent<HealthScript>();

			if(health && health == directHit)
			{
				//took already damage and got a force
				continue;
			}

			if(health && health != directHit && !cachedHealthScripts.Contains(health))
			{
				health.TakeDamage(explosionDamage);
				cachedHealthScripts.Add(health);
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

        RpcExplosion(transform.position, transform.rotation);
        var explosionObject = Instantiate(explosionPrefab, transform.position, transform.rotation);
        RFX4_ColorHelper.ChangeObjectColorByHUE(explosionObject, RFX4_ColorHelper.ColorToHSV(effectColor).H);
        OnCollisionDeactivateBehaviour(false);

        //gameObject.SetActive(false);
        //NetworkServer.UnSpawn(gameObject);
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
        //Instantiate(explosionPrefab,position,rotation);
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