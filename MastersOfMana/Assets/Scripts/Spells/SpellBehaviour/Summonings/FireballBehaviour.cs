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
	public GameObject explosion;
	public float explosionRadius = 4;
	public float explosionTime = 1;
	public float explosionForce = 5;
	public float explosionDamage = 5.0f;

	public TrailRenderer trail;

	private Collider col;

	public float disappearTimer = 3;

    public override void Awake()
    {
        base.Awake();

        if (!mRigid)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException();
        }

        col = GetComponentInChildren<Collider>();
        //col.enabled = false;
    }

    public override void Execute(PlayerScript caster)
    {
        //Get a fireballinstance out of the pool
        GameObject fireball = PoolRegistry.FireballPool.Get();
		FireballBehaviour fb = fireball.GetComponent<FireballBehaviour>();
		       
        //now activate it, so no weird interpolation errors occcure
        //TODO delete this eventually - RPCs are just too slow
        //fireball.GetComponent<A_SummoningBehaviour>().RpcSetActive(true);
        fireball.SetActive(true);
		fb.trail.Clear();

		//position the fireball to 'spawn' at the casters hand, including an offset so it does not collide instantly with the hand
		fb.Reset(caster.handTransform.position + caster.GetAimDirection() * 1.5f, caster.transform.rotation);
		//speed up the fireball to fly into the lookdirection of the player
		fb.mRigid.velocity = caster.GetAimDirection() * mSpeed;

        //create an instance of this fireball on the client's machine
        NetworkServer.Spawn(fireball, PoolRegistry.FireballPool.assetID);

		fb.RpcActivateExplosionMesh(false);

		fb.Disappear();
    }

	void Reset (Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;

		mRigid.Reset();
		mRigid.isKinematic = false;
		col.enabled = true;
	}
		
    protected override void ExecuteCollision_Host(Collision collision) {}

    protected override void ExecuteTriggerEnter_Host(Collider collider)
    //protected override void ExecuteCollision_Host(Collision collision) 
    {

        if (collider.isTrigger)
        {
            return;
        }

		Vector3 directHitForce = mRigid.velocity;

		mRigid.isKinematic = true;
		StartCoroutine(ExplosionEffect());
		//col.enabled = false;

		if(!isServer)
		{
			return;
		}

		HealthScript directHit = collider.gameObject.GetComponentInParent<HealthScript>();
        if (directHit)
        {
            directHit.TakeDamage(mDamage);

			directHitForce.Normalize();
			directHitForce *= explosionForce;

			PlayerScript ps = directHit.GetComponent<PlayerScript>();
            if (ps)
            {
                ps.movement.RpcAddForce(directHitForce, (int)ForceMode.VelocityChange);
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
				
				//check wheather or not we are handling a player or just some random rigidbody
				if (c.attachedRigidbody.CompareTag("Player"))
				{
					PlayerScript ps = c.attachedRigidbody.GetComponent<PlayerScript>();
//					ps.RpcAddExplosionForce(explosionForce, transform.position, explosionRadius);
					Vector3 force = c.attachedRigidbody.transform.TransformPoint(c.attachedRigidbody.centerOfMass) - mRigid.position; 
					force.Normalize();
					force *= explosionForce;
					Debug.DrawRay(c.attachedRigidbody.centerOfMass,force,Color.black,10);

					ps.movement.RpcAddForce(force, (int)ForceMode.VelocityChange);
				}
				else
				{
//					c.attachedRigidbody.AddExplosionForce (explosionForce, transform.position, explosionRadius);
//					UnityEditor.EditorApplication.isPaused = true;
					Vector3 force = c.attachedRigidbody.transform.TransformPoint(c.attachedRigidbody.centerOfMass) - mRigid.position; 
					force.Normalize();
					force *= explosionForce;
					Debug.DrawRay(transform.position,force,Color.white,10);
					c.attachedRigidbody.AddForce(force + Vector3.up, ForceMode.VelocityChange);

				}
			}

            mRigid.velocity = Vector3.zero;
            serverMoveable.RpcStopMotion();
		}
    }

	IEnumerator ExplosionEffect()
	{
		RpcActivateExplosionMesh(true);
		yield return new WaitForSeconds(explosionTime);

		PreventInterpolationIssues();

		gameObject.SetActive(false);
		NetworkServer.UnSpawn(gameObject);
	}

	[ClientRpc]
	void RpcActivateExplosionMesh(bool explosionState)
	{
		explosion.SetActive(explosionState);
		ballMesh.SetActive(!explosionState);
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
		if(explosion != null)
		{
			explosion.transform.localScale = Vector3.one * explosionRadius * 2;
		}
	}
}