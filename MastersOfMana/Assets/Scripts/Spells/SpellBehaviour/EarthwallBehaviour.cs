using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds the nearest object beneath the caster and instantiates a wall, coming out of the floor
/// </summary>
public class EarthwallBehaviour : A_SpellBehaviour {

    //TODO exchange magic numbers with actual data
    public override void Execute(PlayerScript caster)
    {
        RaycastHit hit;
        if (Physics.Raycast(caster.transform.position+0.5f*caster.transform.forward, caster.transform.up*-1, out hit, 50))
        {
            //Instantiate(this, hit.point, caster.transform.rotation);
            GameObject wall = PoolRegistry.EarthwallPool.Get(Pool.Activation.ReturnActivated);
            wall.transform.position = hit.point;
            wall.transform.rotation = caster.transform.rotation;
        }
    }
}