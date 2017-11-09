using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class WhipBehaviour : A_SummoningBehaviour
{
	public float pullForce = 10;
	public float UpForce = 3;

	public float rayRadius = 0.2f;

	public float maxDistance = 30;

	public float disappearTimer = 1.0f;

	public LineRenderer lineRenderer;

    public override void Awake()
    {
        base.Awake();
    }

	[SyncVar(hook = "SetLinePoint0")]
	private Vector3 linePoint0;
	[SyncVar(hook = "SetLinePoint1")]
	private Vector3 linePoint1;

	private void SetLinePoint0(Vector3 vec)
	{
		linePoint0 = vec;
		lineRenderer.SetPosition(0, linePoint0);
	}

	private void SetLinePoint1(Vector3 vec)
	{
		linePoint1 = vec;
		lineRenderer.SetPosition(1, linePoint1);
	}

	public override bool Preview (PlayerScript caster)
	{
		if(!base.Preview (caster))
		{
			return false;
		}

		RaycastHit hit;
		Ray ray = caster.aim.GetCameraRig().GetCenterRay();

		caster.SetColliderIgnoreRaycast(true);
		bool hitSomething = Physics.SphereCast(ray, rayRadius, out hit, maxDistance);
		caster.SetColliderIgnoreRaycast(false);

		if(hitSomething)
		{
			previewIndicator.position = hit.point; 
		}
		else
		{
			previewIndicator.position = OBLIVION;
		}

		return hitSomething;
	}

    public override void Execute(PlayerScript caster)
    {
		WhipBehaviour whipBehaviour = PoolRegistry.WhipPool.Get().GetComponent<WhipBehaviour>();

		whipBehaviour.linePoint0 = caster.handTransform.position;

		RaycastHit hit;
        Ray ray = new Ray(caster.GetCameraPosition(), caster.GetCameraLookDirection());

		caster.SetColliderIgnoreRaycast(true);
		bool hitSomething = Physics.SphereCast(ray, rayRadius, out hit, maxDistance);
		caster.SetColliderIgnoreRaycast(false);

		if(hitSomething)
		{
			whipBehaviour.linePoint1 = hit.point;
		}
		else
		{
			whipBehaviour.linePoint1 = caster.handTransform.position + caster.GetCameraLookDirection() * maxDistance;
		}

        whipBehaviour.gameObject.SetActive(true);
		NetworkServer.Spawn(whipBehaviour.gameObject, PoolRegistry.WhipPool.assetID);

		whipBehaviour.StartCoroutine(whipBehaviour.Done());

		if(hitSomething)
		{
			Vector3 aimDirection = Vector3.Normalize(hit.point - caster.handTransform.position);

			if(hit.collider.attachedRigidbody != null)
			{
				Vector3 force = -aimDirection * pullForce + Vector3.up * UpForce;
			    if (hit.collider.attachedRigidbody.CompareTag("Player"))
			    {
					hit.collider.attachedRigidbody.GetComponent<PlayerScript>().movement.RpcAddForce(force, ForceMode.VelocityChange);
			    }
				else
				{
					hit.collider.attachedRigidbody.AddForce(force, ForceMode.VelocityChange);
				}
			}
			else
			{
				Vector3 forceVector = aimDirection * pullForce + Vector3.up * UpForce;
				caster.movement.RpcAddForce(forceVector, ForceMode.VelocityChange);
			}
		}
    }

//	public void Disappear()
//	{
//		StartCoroutine(Done());
//	}

	public IEnumerator Done()
	{
		yield return new WaitForSeconds(disappearTimer);

		gameObject.SetActive(false);
		NetworkServer.UnSpawn(gameObject);
	}

    protected override void ExecuteCollision_Host(Collision collision) 
	{
    }
}