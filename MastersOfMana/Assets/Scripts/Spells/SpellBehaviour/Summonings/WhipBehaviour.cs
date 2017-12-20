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
    public float hitRadius = 2.0f;

	public float disappearTimer = 1.0f;

	public LineRenderer lineRenderer;

	public PreviewSpell previewPrefab;

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

		RaycastHit hit;
		Ray ray = caster.aim.GetCameraRig().GetCenterRay();

		if(RayCast(caster, ray, out hit))
		{
			previewPrefab.instance.Move(hit.point); 
		}
		else
		{
            previewPrefab.instance.Deactivate();
		}
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        previewPrefab.instance.Deactivate();
	}

    public override void Execute(PlayerScript caster)
    {
		WhipBehaviour whipBehaviour = PoolRegistry.GetInstance(this.gameObject, 4, 4).GetComponent<WhipBehaviour>();

        //initialize the linepoint
        whipBehaviour.linePoint0 = caster.handTransform.position;

        //check for a hit
        var opponents = GameManager.instance.players;
        PlayerScript hitPlayer = null;
        foreach (var p in opponents)
        {
            if (p == caster)
            {
                continue;
            }

            if (ConfirmedHit(p.movement.mRigidbody.worldCenterOfMass, caster, hitRadius, maxDistance))
            {
                hitPlayer = p;
                whipBehaviour.linePoint1 = p.movement.mRigidbody.worldCenterOfMass;
            }
            else if (ConfirmedHit(p.headJoint.position, caster, hitRadius, maxDistance))
            {
                hitPlayer = p;
                whipBehaviour.linePoint1 = p.headJoint.position;
            }
            else if (ConfirmedHit(p.transform.position, caster, hitRadius, maxDistance))
            {
                hitPlayer = p;
                whipBehaviour.linePoint1 = p.transform.position;
            }

            if (hitPlayer)
            {
                break;
            }
        }

        RaycastHit hit;

        //case 1: hit the player
        if (hitPlayer)
        {
            Vector3 aimDirection = Vector3.Normalize(whipBehaviour.linePoint1 - caster.handTransform.position);
            Vector3 force = -aimDirection*pullForce + Vector3.up*UpForce;
            hitPlayer.movement.RpcAddForce(force, ForceMode.VelocityChange);
            hitPlayer.healthScript.TakeDamage(0, GetType());
        }
        //case 2: hit geometry
        else if (RayCast(caster, new Ray(caster.GetCameraPosition(), caster.GetCameraLookDirection()), out hit) && hit.distance <= maxDistance)
        {
            whipBehaviour.linePoint1 = hit.point;
            Vector3 aimDirection = Vector3.Normalize(whipBehaviour.linePoint1 - caster.transform.position);
            Vector3 forceVector = aimDirection * pullForce + Vector3.up * UpForce;
            caster.movement.RpcAddForce(forceVector, ForceMode.VelocityChange);
        }
        //case 3: hit nothing
        else 
        {
            whipBehaviour.linePoint1 = caster.handTransform.position + caster.GetCameraLookDirection() * maxDistance;
        }

        whipBehaviour.gameObject.SetActive(true);
		NetworkServer.Spawn(whipBehaviour.gameObject, whipBehaviour.GetComponent<NetworkIdentity>().assetId);

		whipBehaviour.StartCoroutine(whipBehaviour.DelayedUnspawn());
    }

    public IEnumerator DelayedUnspawn()
	{
		yield return new WaitForSeconds(disappearTimer);

		gameObject.SetActive(false);
		NetworkServer.UnSpawn(gameObject);
	}
}