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

	public GameObject ballMesh;
	public GameObject explosionPrefab;
	public float explosionRadius = 4;
	public float explosionForce = 5;
	public float explosionDamage = 5.0f;

	public TrailRenderer trail;

	public float disappearTimer = 3;

    public override void Awake()
    {
        base.Awake();

        if (!mRigid)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException();
        }
    }

    public override void Execute(PlayerScript caster)
    {
        //Get a fireballinstance out of the pool
		FireballBehaviour fireballBehaviour = PoolRegistry.FireballPool.Get().GetComponent<FireballBehaviour>();
		       
        //now activate it, so no weird interpolation errors occcure
        //TODO delete this eventually - RPCs are just too slow
        //fireball.GetComponent<A_SummoningBehaviour>().RpcSetActive(true);
		fireballBehaviour.gameObject.SetActive(true);
		fireballBehaviour.trail.Clear();

		//position the fireball to 'spawn' at the casters hand, including an offset so it does not collide instantly with the hand
		fireballBehaviour.Reset(caster.handTransform.position + caster.GetAimDirection() * 1.5f, caster.transform.rotation);
		//speed up the fireball to fly into the lookdirection of the player
		fireballBehaviour.mRigid.velocity = caster.GetAimDirection() * mSpeed;

        //create an instance of this fireball on the client's machine
		NetworkServer.Spawn(fireballBehaviour.gameObject, PoolRegistry.FireballPool.assetID);

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
    //protected override void ExecuteCollision_Host(Collision collision) 
    {
        if (collider.isTrigger)
        {
            return;
        }

		Vector3 directHitForce = mRigid.velocity;

		mRigid.isKinematic = true;

		RpcExplosion(transform.position,transform.rotation);
		Instantiate(explosionPrefab,transform.position,transform.rotation);

		if(!isServer)
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
                player.movement.RpcAddForce(directHitForce, (int)ForceMode.VelocityChange);
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

				Vector3 force = c.attachedRigidbody.transform.TransformPoint(c.attachedRigidbody.centerOfMass) - mRigid.position; 
				force.Normalize();
				force *= explosionForce;

				//check wheather or not we are handling a player or just some random rigidbody
				if (c.attachedRigidbody.CompareTag("Player"))
				{
					PlayerScript ps = c.attachedRigidbody.GetComponent<PlayerScript>();
					ps.movement.RpcAddForce(force, (int)ForceMode.VelocityChange);
				}
				else
				{
					c.attachedRigidbody.AddForce(force, ForceMode.VelocityChange);
				}
			}

			gameObject.SetActive(false);
			NetworkServer.UnSpawn(gameObject);
		}
    }

	[ClientRpc]
	void RpcExplosion(Vector3 position, Quaternion rotation)
	{
		Instantiate(explosionPrefab,position,rotation);
	}
		
	public void Disappear()
	{
		StartCoroutine(Done());
	}

	public IEnumerator Done()
	{
		yield return new WaitForSeconds(disappearTimer);

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