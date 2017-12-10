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

	public PreviewSpell previewPrefab;
	private static PreviewSpell sPreview; 

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

	bool RayCast(PlayerScript player, Ray ray, out RaycastHit hit)
	{
		player.SetColliderIgnoreRaycast(true);
		bool hitSomething = Physics.SphereCast(ray, rayRadius, out hit, maxDistance);
		player.SetColliderIgnoreRaycast(false);
		return hitSomething;
	}

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

		if(!sPreview)
		{
			sPreview = GameObject.Instantiate(previewPrefab) as PreviewSpell;
			sPreview.Deactivate();
		}

		RaycastHit hit;
		Ray ray = caster.aim.GetCameraRig().GetCenterRay();

		if(RayCast(caster, ray, out hit))
		{
			sPreview.Move(hit.point); 
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
		//WhipBehaviour whipBehaviour = PoolRegistry.WhipPool.Get().GetComponent<WhipBehaviour>();
		WhipBehaviour whipBehaviour = PoolRegistry.GetInstance(this.gameObject, 4, 4).GetComponent<WhipBehaviour>();

        whipBehaviour.linePoint0 = caster.handTransform.position;

		RaycastHit hit;
        Ray ray = new Ray(caster.GetCameraPosition(), caster.GetCameraLookDirection());

		if(RayCast(caster, ray, out hit))
		{
			whipBehaviour.linePoint1 = hit.point;
		}
		else
		{
			whipBehaviour.linePoint1 = caster.handTransform.position + caster.GetCameraLookDirection() * maxDistance;
		}

        whipBehaviour.gameObject.SetActive(true);
		NetworkServer.Spawn(whipBehaviour.gameObject, whipBehaviour.GetComponent<NetworkIdentity>().assetId);

		whipBehaviour.StartCoroutine(whipBehaviour.Done());

		if(hit.collider != null)
		{
			Vector3 aimDirection = Vector3.Normalize(hit.point - caster.handTransform.position);

			if(hit.collider.attachedRigidbody != null)
			{
				Vector3 force = -aimDirection * pullForce + Vector3.up * UpForce;
			    if (hit.collider.attachedRigidbody.CompareTag("Player"))
			    {
                    var ps = hit.collider.attachedRigidbody.GetComponent<PlayerScript>();
					ps.movement.RpcAddForce(force, ForceMode.VelocityChange);
                    ps.healthScript.TakeDamage(0, GetType());
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