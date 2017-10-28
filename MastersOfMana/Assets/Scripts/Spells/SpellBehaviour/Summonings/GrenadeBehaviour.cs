using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// The specific behaviour of the fireball, that is manifested in the scene
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GrenadeBehaviour : A_SummoningBehaviour
{
	public float throwForce = 20;
	public float lifeTime = 3;
	public float explosionForce = 600;
	public float explosionRadius = 7;
	public float damage = 10;
	public float explosionTime = 0.5f;

	public GameObject explosion;
	public GameObject grenadeMesh;

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
		GameObject grenade = PoolRegistry.GrenadePool.Get();

		GrenadeBehaviour grenadeBehaviour = grenade.GetComponent<GrenadeBehaviour>();
		grenade.SetActive(true);
		//create an instance of this fireball on the client's machine
		NetworkServer.Spawn(grenade, PoolRegistry.GrenadePool.assetID);

		grenadeBehaviour.Reset(caster.handTransform.position + caster.GetAimDirection() * 1.5f, caster.transform.rotation);

		//speed up the fireball to fly into the lookdirection of the player
		grenadeBehaviour.Shoot(caster.GetAimDirection());

    }

	#region implemented abstract members of A_SummoningBehaviour

	protected override void ExecuteCollision_Host (Collision collision)
	{
	}

	#endregion

	void Reset (Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;

		mRigid.Reset();
		mRigid.isKinematic = false;

		RpcActivateExplosionMesh(false);
	}

	void Shoot(Vector3 direction)
	{
		mRigid.velocity = direction * throwForce;
		StartCoroutine(LightFuse(lifeTime));
	}

	IEnumerator LightFuse(float time)
	{
		yield return new WaitForSeconds(time);
		Explode ();
	}

	void Explode ()
	{
		mRigid.isKinematic = true;

		StartCoroutine(ExplosionEffect());


		if(!isServer)
		{
			return;
		}

		Collider[] cols = Physics.OverlapSphere (mRigid.position, explosionRadius);

		List<HealthScript> cachedHealthScripts = new List<HealthScript>(cols.Length);
		List<Rigidbody> cachedRigidbodies = new List<Rigidbody>(cols.Length);

		foreach (Collider c in cols) 
		{
			if (c.attachedRigidbody && !cachedRigidbodies.Contains(c.attachedRigidbody)) 
			{
				cachedRigidbodies.Add(c.attachedRigidbody);

                //check wheather or not we are handling a player or just some random rigidbody
			    if (c.attachedRigidbody.CompareTag("Player"))
			    {
			        PlayerScript ps = c.attachedRigidbody.GetComponent<PlayerScript>();
//                  ps.RpcAddExplosionForce(explosionForce, transform.position, explosionRadius);
					Vector3 force = c.attachedRigidbody.transform.TransformPoint(c.attachedRigidbody.centerOfMass) - mRigid.position; 
					force.Normalize();
					force *= explosionForce;
					ps.movement.RpcAddForce(force, (int)ForceMode.VelocityChange);

			    }
			    else
			    {
//                  c.attachedRigidbody.AddExplosionForce (explosionForce, transform.position, explosionRadius);
					Vector3 force = c.attachedRigidbody.transform.TransformPoint(c.attachedRigidbody.centerOfMass) - mRigid.position; 
					force.Normalize();
					force *= explosionForce;
					c.attachedRigidbody.AddForce(force, ForceMode.VelocityChange);
			    }
			}
				
			HealthScript health = c.GetComponentInParent<HealthScript>();
			if(health && !cachedHealthScripts.Contains(health))
			{
				health.TakeDamage(damage);
				cachedHealthScripts.Add(health);
			}
		}
	}

	[ClientRpc]
	void RpcActivateExplosionMesh(bool explosionState)
	{
		explosion.SetActive(explosionState);
		grenadeMesh.SetActive(!explosionState);
	}

	IEnumerator ExplosionEffect()
	{
		RpcActivateExplosionMesh(true);
		yield return new WaitForSeconds(explosionTime);
		RpcSetActive(false);
	}

	public void Deactivate()
	{
		if(isServer)
		{
			gameObject.SetActive(false);
			NetworkServer.UnSpawn(gameObject);
		}
	}

	void OnValidate()
	{
		if(explosion != null)
		{
			explosion.transform.localScale = Vector3.one * explosionRadius * 2;
		}
	}
}