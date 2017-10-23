using System;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(LineRenderer))]
public class WhipBehaviour : A_SummoningBehaviour
{
    public float damage = 5.0f;
	public float pullHitForce = 10;
	public float pullHitUpForce = 3;

	public float pullPlayerForce = 10;
	public float pullPlayerUpForce = 3;

	public float maxDistance = 20;

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
		lineRenderer.SetPosition(0, vec);
	}

	private void SetLinePoint1(Vector3 vec)
	{
		lineRenderer.SetPosition(1, vec);
	}

    public override void Execute(PlayerScript caster)
    {
		GameObject whip = PoolRegistry.WhipPool.Get();
		WhipBehaviour whipBehaviour = whip.GetComponent<WhipBehaviour>();

		whipBehaviour.linePoint0 = caster.handTransform.position;

		RaycastHit hit;
        Ray ray = new Ray(caster.GetCameraPosition(), caster.GetCameraLookDirection());
		bool hitSomething = Physics.Raycast(ray, out hit, maxDistance);

		if(hitSomething)
		{
			whipBehaviour.linePoint1 = hit.point;
		}
		else
		{
			whipBehaviour.linePoint1 = caster.handTransform.position + caster.GetAimDirection() * maxDistance;
		}

        whip.SetActive(true);
		NetworkServer.Spawn(whip, PoolRegistry.WhipPool.assetID);

//		if(!isServer)
//		{
//			return;
//		}

		if(hitSomething)
		{
			HealthScript hitHealth = hit.transform.GetComponentInChildren<HealthScript>();
			if(hitHealth != null)
			{
				hitHealth.TakeDamage(damage);
			}

			if(hit.collider.attachedRigidbody != null)
			{
			    Vector3 force = -caster.GetAimDirection()*pullHitForce + Vector3.up*pullHitUpForce;
			    if (hit.collider.attachedRigidbody.CompareTag("Player"))
			    {
			        hit.collider.attachedRigidbody.GetComponent<PlayerScript>().RpcAddForce(force, (int) ForceMode.Impulse);
			    }

				hit.collider.attachedRigidbody.AddForce(force, ForceMode.Impulse);
			}

			caster.RpcAddForce(caster.GetAimDirection() * pullPlayerForce + Vector3.up * pullPlayerUpForce, (int)ForceMode.Impulse);
		}
    }

    protected override void ExecuteCollision_Host(Collision collision) 
	{
    }
}