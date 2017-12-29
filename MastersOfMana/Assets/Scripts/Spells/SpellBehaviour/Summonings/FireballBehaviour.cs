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
	[Tooltip("This value should be the same as the colliders radius. kkthxbye")]
	public float ballRadius = 0.25f;

	[Header("Direct Hits")]
    public int directDamage = 8;
	public float directForce = 4;

	[Header("Explosion")]
	public Color effectColor;
	public GameObject explosionPrefab;
	public ExplosionFalloff explosionFalloff;
	public float explosionRadius = 4;

	[Space()]
	public float disappearTimer = 3;

    public GameObject[] DeactivatedObjectsOnCollision;

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

        preview.instance.SetAvailability(caster.CurrentSpellReady());

		RaycastHit hit;
		if(caster.HandTransformIsObscured(out hit))
		{
            preview.instance.MoveAndRotate(hit.point, caster.aim.currentLookRotation);
			return;
		}
	
		caster.SetColliderIgnoreRaycast(true);
		if(Physics.CheckSphere(caster.handTransform.position, ballRadius))
		{
			//this is only reset here, because the aimdirection will also set the ignore layer
			caster.SetColliderIgnoreRaycast(false);
            preview.instance.MoveAndRotate(caster.handTransform.position, caster.aim.currentLookRotation);
			return;
		}

		Vector3 aimDirection = GetAimClient(caster, out hit);
	
        caster.SetColliderIgnoreRaycast(true);
		if(Physics.SphereCast(caster.handTransform.position, ballRadius, aimDirection, out hit))
		{
            preview.instance.MoveAndRotate(hit.point + hit.normal * ballRadius, Quaternion.LookRotation(hit.normal));
		}
		else
		{
            if(Physics.CheckSphere(caster.handTransform.position, ballRadius))
			{
                preview.instance.MoveAndRotate(caster.handTransform.position, caster.aim.currentLookRotation);
			}
			else
			{
				preview.instance.Deactivate();
			}
        }
		caster.SetColliderIgnoreRaycast(false);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
		preview.instance.Deactivate();
	}

    public override void Execute(PlayerScript caster)
    {
		Vector3 aimDirection = GetAimServer(caster); 

		Vector3 initPos;
		RaycastHit hit;

		if(caster.HandTransformIsObscured(out hit))
		{
			Vector3 back = caster.handTransform.parent.position - caster.handTransform.position;
			back.Normalize();
			initPos = hit.point + back * ballRadius;
			aimDirection = caster.aim.currentLookRotation * Vector3.forward;
		}
		else
		{
			initPos = caster.handTransform.position;
		}
			
        //Get a fireballinstance out of the pool
		FireballBehaviour fireballBehaviour = PoolRegistry.GetInstance(gameObject, initPos, caster.transform.rotation, 5, 5).GetComponent<FireballBehaviour>();
		fireballBehaviour.caster = caster;
		fireballBehaviour.mLastPosition = initPos;

        //now activate it, so no weird interpolation errors occcure
        fireballBehaviour.gameObject.SetActive(true);


		//position the fireball to 'spawn' at the casters hand
		fireballBehaviour.Reset();
		//speed up the fireball to fly into the lookdirection of the player
		fireballBehaviour.mRigid.velocity = aimDirection * speed;

        OnCollisionDeactivateBehaviour(true);

        //create an instance of this fireball on the client's machine
        NetworkServer.Spawn(fireballBehaviour.gameObject, fireballBehaviour.GetComponent<NetworkIdentity>().assetId);

		fireballBehaviour.StartCoroutine(fireballBehaviour.Done());
    }

	private Vector3 mLastPosition;

	void LateUpdate()
	{
		mLastPosition = mRigid.position;
	}

	void Reset ()
	{
		mRigid.Reset();
		mRigid.isKinematic = false;
		
		mTriggerEnabled = true;
	}

	private bool mTriggerEnabled = true;

    protected override void ExecuteTriggerEnter_Host(Collider collider)
    {
		if(!mTriggerEnabled)
		{
			return;
		}

		//skip trigger and the casters collision boxes
        if (collider.isTrigger || (caster && caster.IsColliderPartOf(collider)))
        {
            return;
        }

		Vector3 directHitForce = mRigid.velocity;
		mRigid.isKinematic = true;

		//since the fireball is very fast, the hit detection tend to be too slow
		RaycastHit adjustmentHit;
		if(Physics.Linecast(mLastPosition, mRigid.position, out adjustmentHit))
		{
			//technically the normal is not correct, but it looks fine and is only really wrong when the angle is super flat
			mRigid.position = adjustmentHit.point - (mRigid.position - mLastPosition) * ballRadius;
		}
		
		RpcExplosion(mRigid.position, mRigid.rotation);

		//disallow a double trigger when touching multiple collider
		mTriggerEnabled = false;

        HealthScript directHit = null;

		//only hurt non static objects
        if(!collider.gameObject.isStatic)
        {
            directHit = collider.gameObject.GetComponentInParent<HealthScript>();
            if (directHit)
            {
				//apply direct hit damage
				directHit.TakeDamage(directDamage, GetType());

				//if it was a moveable object, apply a force
				ServerMoveable moveable = directHit.GetComponentInParent<ServerMoveable>();
				if (moveable)
                {
                    directHitForce.Normalize();
                    directHitForce *= directForce;
					moveable.RpcAddForce(directHitForce, ForceMode.VelocityChange);
                }
            }
        }

		//explosion force and damage:
		ExplosionDamage(mRigid.position, explosionRadius, explosionFalloff, new List<HealthScript>{directHit} );
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

    public IEnumerator Done()
	{
		yield return new WaitForSeconds(disappearTimer);

        OnCollisionDeactivateBehaviour(true);
        gameObject.SetActive(false);
		NetworkServer.UnSpawn(gameObject);
	}
}