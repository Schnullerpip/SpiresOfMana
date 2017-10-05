using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds the nearest object beneath the caster and instantiates a wall, coming out of the floor
/// </summary>
public class EarthwallBehaviour : A_SummoningBehaviour {

    //TODO exchange magic numbers with actual data
    public override void Execute(PlayerScript caster)
    {
        RaycastHit hit;
        if (Physics.Raycast(caster.transform.position+0.8f*(caster.transform.forward + caster.transform.up), caster.transform.up*-1, out hit, 50))
        {
            GameObject wall = PoolRegistry.EarthwallPool.Get(Pool.Activation.ReturnActivated);
            wall.transform.position = hit.point+Vector3.up*1.0f;
            wall.transform.rotation = caster.transform.rotation;
        }
    }

    protected override void ExecuteCollisionOnServer(Collision collision)
    {
        //should a collision with a wall really do anything?
    }
}