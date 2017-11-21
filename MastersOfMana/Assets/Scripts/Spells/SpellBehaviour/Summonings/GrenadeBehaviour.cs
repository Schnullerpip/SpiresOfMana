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
	public int damage = 10;
	public float explosionTime = 0.5f;

	public GameObject explosionPrefab;
	public GameObject grenadeMesh;

	public PreviewSpellTrajectory previewPrefab;
	private static float? sRigidMass = null;

//    private Rigidbody mStickTo = null;
//    private Vector3 mStickPosition;

    public override void Awake()
    {
        base.Awake();
        if (!mRigid)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException();
        }
    }

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

		if(sRigidMass == null)
		{
			sRigidMass = GetComponent<Rigidbody>().mass;
		}

		Vector3 vel = GetAimLocal(caster) * throwForce;

//		sPreview.Move(caster.transform.position + caster.transform.forward * 3);
		(previewPrefab.instance as PreviewSpellTrajectory).VisualizeTrajectory(caster.handTransform.position, vel, sRigidMass.Value);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
		previewPrefab.instance.Deactivate();

	}

    public override void Execute(PlayerScript caster)
    {
		//GrenadeBehaviour grenadeBehaviour = PoolRegistry.GrenadePool.Get().GetComponent<GrenadeBehaviour>();
		GrenadeBehaviour grenadeBehaviour = PoolRegistry.Instantiate(this.gameObject).GetComponent<GrenadeBehaviour>();
        //create an instance of this grenade on the client's machine

        grenadeBehaviour.gameObject.SetActive(true);

		Vector3 aimDirection = GetAim(caster);
		
		grenadeBehaviour.Reset(caster.handTransform.position + aimDirection, caster.transform.rotation);
		grenadeBehaviour.mRigid.velocity = aimDirection * throwForce;

		NetworkServer.Spawn(grenadeBehaviour.gameObject, grenadeBehaviour.GetComponent<NetworkIdentity>().assetId);

		grenadeBehaviour.LightFuse(lifeTime);
    }

//    public void Update()
//    {
//        if (isServer && mStickTo)
//        {
//            Vector3 moveCorrection = mStickTo.transform.position - mStickPosition;
//            mStickPosition = mRigid.position;
//            mRigid.MovePosition(mRigid.position + moveCorrection);
//            Debug.Log("correcting position from : " + mStickPosition + " to: " + mRigid.position);
//        }
//    }

	#region implemented abstract members of A_SummoningBehaviour

	protected override void ExecuteCollision_Host (Collision collision)
	{
//	    if (!mStickTo)
//	    {
//	        var rigid = collision.collider.attachedRigidbody;
//	        if (rigid)
//	        {
//	            mStickTo = rigid;
//                Debug.Log("sticking to " + rigid.name);
//	            mRigid.useGravity = false;
//	            mRigid.isKinematic = true;
//
//	            grenadeMesh.GetComponent<Collider>().enabled = false;
//
//	            mStickPosition = mRigid.position;
//	        }
//	    }
	}

	#endregion

	void Reset (Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;

		mRigid.Reset();
		mRigid.isKinematic = false;
	}

	private void LightFuse(float time)
	{
		StartCoroutine(C_LightFuse(time));
	}

	IEnumerator C_LightFuse(float time)
	{
		//TODO cache waitforseconds
		yield return new WaitForSeconds(time);
		Explode ();
	}

	void Explode ()
	{
		mRigid.isKinematic = true;

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
				
			HealthScript health = c.GetComponentInParent<HealthScript>();
			if(health && !cachedHealthScripts.Contains(health))
			{
				health.TakeDamage(damage);
				cachedHealthScripts.Add(health);
			}
		}

		RpcExplosion(transform.position,transform.rotation);
        //Instantiate(explosionPrefab,transform.position,transform.rotation);

        StartCoroutine(DestroyNextFrame());
    }

	[ClientRpc]
	void RpcExplosion(Vector3 position, Quaternion rotation)
	{
        Instantiate(explosionPrefab,position,rotation);
	}

    public IEnumerator DestroyNextFrame()
    {
        yield return 0;//wait 1 frame

        NetworkServer.UnSpawn(gameObject);
        gameObject.SetActive(false);
    }

	void OnValidate()
	{
		if(explosionPrefab != null)
		{
			explosionPrefab.transform.localScale = Vector3.one * explosionRadius * 2;
		}
	}
}