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

    [SerializeField] private float mMaxDistance;

    [SerializeField] private float mOffsetToPlayer;

    [SerializeField] private float mCapsuleHeight;

    [SerializeField] private float mCapsuleRadius;

    [SerializeField] private float mPushForce;

    public override void Execute(PlayerScript caster)
    {
        //properties the spellneeds
        Vector3 point1, point2, originalPosition = caster.transform.position, direction = caster.GetAimDirection();

        point1 = point2 = caster.headJoint.position + direction*mOffsetToPlayer;
        point2 -= new Vector3(0, mCapsuleHeight, 0);

        //capsulecast to find new position
        RaycastHit hit;
        bool hitSomething = Physics.CapsuleCast(point1, point2, mCapsuleRadius, direction, out hit, mMaxDistance);

        //find position of player to that position
        //if the way is free go until maxdistance is reached - else go until the hit object
        Vector3 newPosition = originalPosition + direction*(hitSomething ? hit.distance : mMaxDistance);

        if (hitSomething)
        {
            PlayerScript ps = hit.collider.GetComponentInParent<PlayerScript>();
            if (ps)
            {
                ps.movement.RpcAddForce(direction*mPushForce, ForceMode.Impulse);
            }
        }

        //cast a trail or something so the enemy does not recognize the dash as a glitch
        GameObject ds = PoolRegistry.DashTrailPool.Get();
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
}