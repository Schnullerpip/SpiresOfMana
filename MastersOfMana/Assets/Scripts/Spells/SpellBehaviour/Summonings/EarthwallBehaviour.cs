using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Finds the nearest object beneath the caster and instantiates a wall, coming out of the floor
/// </summary>
public class EarthwallBehaviour : A_SummoningBehaviour {

    //TODO exchange magic numbers with actual data
    public override void Execute(PlayerScript caster)
    {
        RaycastHit hit;
        if (Physics.Raycast(caster.transform.position + 0.8f * (caster.transform.forward + caster.transform.up), caster.transform.up * -1, out hit, 50))
        {
            //GameObject wall = PoolRegistry.EarthwallPool.Get(Pool.Activation.ReturnActivated);
            GameObject wall = PoolRegistry.instance.Instantiate(this.gameObject);
            wall.SetActive(true);
            wall.transform.position = hit.point;
            wall.transform.rotation = caster.transform.rotation;
            NetworkServer.Spawn(wall);
        }
    }

    protected override void ExecuteCollision_Host(Collision collision)
    {
        //should a collision with a wall really do anything?
    }
}