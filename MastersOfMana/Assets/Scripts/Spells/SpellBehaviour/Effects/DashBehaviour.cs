using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Specifies, how the dash spell works 
/// it takes the casters original speed and enhances it for a distinct period of time, then reverts the speed value of the player to the previously cached value
/// !!! THIS COULD BE BETTER -> there probably should be an original speed constant in the playerscript class that is referenced instead of caching a value on the effect start
/// </summary>
public class DashBehaviour : A_EffectBehaviour
{

//    [SerializeField] private float mMaxDistance;
//
//    [SerializeField] private float mOffsetToPlayer;
//
//    [SerializeField] private float mCapsuleHeight;
//
//    [SerializeField] private float mCapsuleRadius;
//
//    [SerializeField] private float mPushForce;
//
//    [SerializeField] private float mReduction;

	public float maxDistance = 10;
	public float pushForce = 10;

	public float playerRadius = 0.35f;
	public float playerHeight = 2;

	[Tooltip("Additional height for the lower sphere of the capsule. This is used to effectively 'pull in' the feet, so the character doesn't get stuck so easily on the ground.")]
	public float shortening = 0.3f;

	[Tooltip("This is the distance the lower sphere of the capsule is pushed forwards to handle straight walls better. This should be rather low")]
	public float feetAdvancement = 0.1f;

	[Tooltip("The offset of the capsule to handle standing very close to a wall. This value describes the offset of the capsule backwards. This has to be smaller than the radius.")]
	public float backwardOffset = 0.02f;

	public PreviewSpell previewPrefab;
	private static PreviewSpell sPreview; 

	Vector3 GetNewPosition (PlayerScript caster, Vector3 direction, out RaycastHit hit)
	{
		/*
		//properties the spellneeds
		Vector3 point1, point2;
		Vector3 originalPosition = caster.transform.position;

		point1 = point2 = caster.handTransform.position + direction*mOffsetToPlayer;
		point2 -= new Vector3(0, mCapsuleHeight, 0);

		//capsulecast to find new position
		bool hitSomething = Physics.CapsuleCast(point1, point2, mCapsuleRadius, direction, out hit, mMaxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

		//find position of player to that position
		//if the way is free go until maxdistance is reached - else go until the hit object
		return originalPosition + direction*(hitSomething ? hit.distance-mReduction : mMaxDistance);
		*/

//		if(Physics.SphereCast(caster.handTransform.position, playerRadius, direction, out hit, mMaxDistance))
//		{
//			return hit.point + hit.normal * Vector3.Angle(hit.normal, Vector3.up) / 90 * .35f;
//		}

		Vector3 lowerPoint = caster.transform.position + Vector3.up * (playerRadius + shortening) - caster.transform.forward * backwardOffset;
		Vector3 upperPoint = caster.transform.position + Vector3.up * playerHeight + Vector3.down * playerRadius - caster.transform.forward * (feetAdvancement + backwardOffset);

		Debug.DrawRay(lowerPoint + Vector3.down * playerRadius, direction * maxDistance,Color.yellow);
		Debug.DrawRay(lowerPoint, direction * maxDistance);
		Debug.DrawRay(upperPoint, direction * maxDistance);
		Debug.DrawRay(upperPoint + Vector3.up * playerRadius, direction * maxDistance,Color.yellow);

		if(Physics.CapsuleCast(lowerPoint,upperPoint,playerRadius, direction, out hit, maxDistance))
		{
			float surfaceAngle = Vector3.Angle(hit.normal, Vector3.up);

			if(surfaceAngle > 90)
			{
				Debug.Log("Ceiling");

				RaycastHit ceilingHit;

				if(Physics.Raycast(hit.point, hit.normal, out ceilingHit, playerHeight))
				{
					return hit.point + hit.normal * (surfaceAngle / 90) * playerRadius;
				}
				else
				{
					return caster.transform.position + (direction * hit.distance);
				}
			} 
			else 
			{
				//place the player to the hit point, but add the normal of the surface with a weighting depending on the angle of the surface.
				//a wall should add the full player's radius, placing him/her right next to wall instead of into it 
				return hit.point + hit.normal * (surfaceAngle / 90) * playerRadius;

			}
		}
		else
		{
			RaycastHit hitDown;
			if(Physics.SphereCast(lowerPoint + direction * maxDistance, playerRadius, Vector3.down, out hitDown, playerRadius + shortening))
			{
				return hitDown.point;
			}
			else
			{
				return caster.transform.position + (direction * maxDistance) + Vector3.down * playerRadius;
			}
		}
	}
		
	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

		if(!sPreview)
		{
			sPreview = GameObject.Instantiate(previewPrefab) as PreviewSpell;
		}

		Vector3 direction = caster.aim.currentLookRotation * Vector3.forward;

		RaycastHit hit;

		Vector3 newPosition = GetNewPosition(caster, direction, out hit);

		sPreview.MoveAndRotate(newPosition, caster.transform.rotation);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
		if(sPreview)
		{
			sPreview.Deactivate();
		}
	}

    public GameObject dashEffectPrefab; 

    public override void Execute(PlayerScript caster)
    {
		Vector3 direction = caster.aim.currentLookRotation * Vector3.forward;

		RaycastHit hit;

		Vector3 newPosition = GetNewPosition(caster, direction, out hit);

		if (hit.collider != null)
        {
            PlayerScript ps = hit.collider.GetComponentInParent<PlayerScript>();
            if (ps)
            {
                Vector3 pushDirection = Vector3.Normalize(ps.transform.TransformPoint(ps.movement.mRigidbody.centerOfMass) - caster.transform.position);
                ps.movement.RpcSetVelocity(pushDirection*pushForce);
            }
        }

        //cast a trail or something so the enemy does not recognize the dash as a glitch
        GameObject ds = PoolRegistry.Instantiate(dashEffectPrefab);
        //position the trail
        Vector3 pos = caster.transform.position;
        pos.y += 1;
        ds.transform.position = pos;
        ds.transform.rotation = caster.headJoint.transform.rotation;
        //activate the trail on all clients
        ds.SetActive(true);
        NetworkServer.Spawn(ds);

        //set the new position of the player
        caster.movement.RpcSetPosition(newPosition);

        //make the trail disappear after one second
        caster.StartCoroutine(UnspawnTrail(ds));
    }

    public IEnumerator UnspawnTrail(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.SetActive(false);
        NetworkServer.UnSpawn(obj);
    }

	void OnValidate()
	{
		backwardOffset = Mathf.Clamp(backwardOffset, 0, playerRadius);
	}
}