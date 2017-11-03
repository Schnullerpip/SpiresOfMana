using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class WhipBehaviour : A_SummoningBehaviour
{
	public float pullHitForce = 10;
	public float pullHitUpForce = 3;

	public float rayRadius = 0.2f;

	public float pullPlayerForce = 10;
	public float pullPlayerUpForce = 3;

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

    public override void Execute(PlayerScript caster)
    {
		GameObject whip = PoolRegistry.WhipPool.Get();
		WhipBehaviour whipBehaviour = whip.GetComponent<WhipBehaviour>();

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

        whip.SetActive(true);
		NetworkServer.Spawn(whip, PoolRegistry.WhipPool.assetID);

		whipBehaviour.Disappear();

		if(hitSomething)
		{
			Vector3 aimDirection = Vector3.Normalize(hit.point - caster.handTransform.position);

			if(hit.collider.attachedRigidbody != null)
			{
				Vector3 force = -aimDirection * pullHitForce + Vector3.up*pullHitUpForce;
			    if (hit.collider.attachedRigidbody.CompareTag("Player"))
			    {
			        hit.collider.attachedRigidbody.GetComponent<PlayerScript>().movement.RpcAddForce(force, ForceMode.Impulse);
			    }
				else
				{
					hit.collider.attachedRigidbody.AddForce(force, ForceMode.Impulse);
				}
			}

			Vector3 forceVector = aimDirection * pullPlayerForce + Vector3.up * pullPlayerUpForce;

			Debug.DrawRay(caster.transform.position, forceVector, Color.red, 10);
			caster.movement.RpcAddForce(forceVector, ForceMode.Impulse);
		}

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

    protected override void ExecuteCollision_Host(Collision collision) 
	{
    }
}