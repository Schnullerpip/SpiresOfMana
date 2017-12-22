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
	public float maxDistance = 10;
	public float pushForce = 10;
    public float pushRadius = 2;

	public float playerRadius = 0.35f;
	public float playerHeight = 2;

	[Tooltip("Additional height for the lower sphere of the capsule. This is used to effectively 'pull in' the feet, so the character doesn't get stuck so easily on the ground.")]
	public float upwardsOffset = 1.0f;

	[Tooltip("This is the distance the lower sphere of the capsule is pushed forwards to handle straight walls better. This should be rather low")]
	public float feetAdvancement = 0.1f;

    //spawnable effectprefabs
    public GameObject dashEffectPrefab;
    public GameObject dashBlastEffect;

	Vector3 GetNewPosition (PlayerScript player, Vector3 direction, out RaycastHit hit)
	{
        //when the player is up against a wall
        if(player.HandTransformIsObscured())
        {
            hit = new RaycastHit();
            //return the current position and do nothing
            return player.transform.position;
        }

        //temporaraly disable the colliders
        player.SetColliderIgnoreRaycast(true);

        //lower and upper point that define the capsule cast
        Vector3 lowerPoint = player.transform.position //base position
                                   + Vector3.up * (playerRadius + upwardsOffset); //offset of the capsule radius to get to the center of the lower sphere

        Vector3 upperPoint = player.transform.position //base position
                                   + Vector3.up * (playerHeight + upwardsOffset) //add the height to get to the top
                                   + Vector3.down * playerRadius //go down the radius to get to the center of the sphere that defines the capsule
                                   - player.transform.forward * feetAdvancement; //subtract the feetadvance in order to make sure the feet touch a vertical wall first

        //Debug.DrawRay(lowerPoint + Vector3.down * playerRadius, direction * maxDistance,Color.yellow);
        //Debug.DrawRay(lowerPoint, direction * maxDistance);
        //Debug.DrawRay(upperPoint, direction * maxDistance);
        //Debug.DrawRay(upperPoint + Vector3.up * playerRadius, direction * maxDistance,Color.yellow);

		Vector3 returnPoint;

        //case 1: the capsule cast hit something
        if(Physics.CapsuleCast(lowerPoint, upperPoint, playerRadius, direction, out hit, maxDistance))
		{
            //Debug.DrawRay(hit.point, hit.normal, Color.cyan);

			float surfaceAngle = Vector3.Angle(hit.normal, Vector3.up);

            //case 1.1: check for potential ceilings and overhangs. 95 because some walls could be 91-ish and therefore ruin the assumptions
			if(surfaceAngle > 95)
			{
                Vector3 offset = -direction * playerRadius //go back a bit
                    + Vector3.down * (playerHeight - playerRadius); //go down the height and radius, since the capsule hits earlier than the head
				//Debug.DrawRay(hit.point, offset, Color.red);
                returnPoint = hit.point + offset;
			} 
            //case 1.2: the hit surface is a wall or the ground
			else 
			{
				//place the player to the hit point, but add the normal of the surface with a weighting depending on the angle of the surface.
				//a wall should add the full player's radius, placing him/her right next to wall instead of into it 
                returnPoint = hit.point + hit.normal * (surfaceAngle / 90) * playerRadius;
			}
		}
        //case 2: ray ends in midair
		else
		{
			RaycastHit hitDown;
            //case 2.1: the ground is nearby
			if(Physics.SphereCast(lowerPoint + direction * maxDistance, playerRadius, Vector3.down, out hitDown, playerRadius + upwardsOffset))
			{
                returnPoint = hitDown.point;
			}
            //case 2.2: no ground near
			else
			{
                //just move in the aimed direction, max distance
                returnPoint = player.transform.position + (direction * maxDistance) + Vector3.down * playerRadius;
			}
		}

        //enable collision again!
        player.SetColliderIgnoreRaycast(false);
        return returnPoint;
	}
		
	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

        Vector3 direction = caster.aim.currentLookRotation * Vector3.forward;

		RaycastHit hit;

		Vector3 newPosition = GetNewPosition(caster, direction, out hit);

		preview.instance.MoveAndRotate(newPosition, caster.transform.rotation);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);

		preview.instance.Deactivate();

	}

    public override void Execute(PlayerScript caster)
    {
		Vector3 direction = caster.Client_GetCurrentLookDirection() * Vector3.forward;

		RaycastHit hit;

        Vector3 newPosition = GetNewPosition(caster, direction, out hit);

        //TODO should direct hit be stronger? else the collective push force handles this later in IterateCollidersAndApply
		//if (hit.collider != null)
  //      {
            //PlayerScript opponent = hit.collider.GetComponentInParent<PlayerScript>();
            //if (opponent && opponent != caster)
            //{
                //Vector3 pushVector = opponent.movement.mRigidbody.worldCenterOfMass - newPosition;
                //Vector3 pushDirection = Vector3.Normalize(pushVector);
                //opponent.healthScript.TakeDamage(0, this.GetType());
                //opponent.movement.RpcSetVelocityAndMovePosition(pushDirection * pushForce, newPosition + pushVector - opponent.movement.mRigidbody.centerOfMass);
            //}
        //}

        //cast a trail or something so the enemy does not recognize the dash as a glitch
        GameObject ds = PoolRegistry.GetInstance(dashEffectPrefab, 4, 4);
        //position the trail
        Vector3 pos = caster.transform.position;
        pos.y += 1;
        ds.transform.position = pos;
        ds.transform.rotation = caster.headJoint.transform.rotation;

        //activate the trail on all clients
        ds.SetActive(true);
        NetworkServer.Spawn(ds);

        //set the new position of the player
        caster.movement.RpcMovePosition(newPosition);

        //push objects near the caster toward his direction (softly - should not replace windwall)
        IterateCollidersAndApply(Physics.OverlapSphere(newPosition, pushRadius), (ServerMoveable sm) =>
        {
            if (sm != caster.movement)
            {
                Vector3 pushVector = sm.mRigidbody.worldCenterOfMass - newPosition;
                Vector3 pushDirection = Vector3.Normalize(pushVector);
                sm.RpcSetVelocityAndMovePosition(pushDirection * pushForce, newPosition + pushVector - sm.mRigidbody.centerOfMass);

                //if its a player, affect it (for statscollection)
                PlayerScript ps = sm.GetComponentInParent<PlayerScript>();
                if (ps)
                {
                    ps.healthScript.TakeDamage(0, GetType());
                }
            }
        });

        //fire the blastParticleEffect
        GameObject pe = PoolRegistry.GetInstance(dashBlastEffect, newPosition + Vector3.up, Quaternion.identity, 4, 4);
        pe.gameObject.SetActive(true);
        NetworkServer.Spawn(pe.gameObject);

        //make the trail disappear after one second
        caster.StartCoroutine(Unspawn(ds, pe));
    }

    public IEnumerator Unspawn(GameObject trail, GameObject blastEffect)
    {
        yield return new WaitForSeconds(1);
        trail.SetActive(false);
        blastEffect.SetActive(false);

        NetworkServer.UnSpawn(trail);
        NetworkServer.UnSpawn(blastEffect);
    }
}