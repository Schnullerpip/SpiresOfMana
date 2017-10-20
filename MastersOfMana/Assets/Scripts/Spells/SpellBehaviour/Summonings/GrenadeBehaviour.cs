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

	public DelayedAction explosion;
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

		grenadeBehaviour.Reset(caster.handTransform.position + caster.GetAimDirection() * 1.5f, caster.transform.rotation);

		//speed up the fireball to fly into the lookdirection of the player
		grenadeBehaviour.Shoot(caster.GetAimDirection());

		//create an instance of this fireball on the client's machine
		NetworkServer.Spawn(grenade, PoolRegistry.GrenadePool.assetID);
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
		explosion.gameObject.SetActive (false);
		grenadeMesh.SetActive (true);
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
		explosion.gameObject.SetActive(true);
		explosion.InvokeActions();
		grenadeMesh.SetActive(false);

		if(!isServer)
		{
			return;
		}

		Collider[] cols = Physics.OverlapSphere (transform.position, explosionRadius);
		foreach (Collider c in cols) 
		{
			if (c.attachedRigidbody) 
			{
				c.attachedRigidbody.AddExplosionForce (explosionForce, transform.position, explosionRadius);
			}
			HealthScript health = c.GetComponentInParent<HealthScript>();
			if(health != null)
			{
				health.TakeDamage(damage);
			}
		}
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