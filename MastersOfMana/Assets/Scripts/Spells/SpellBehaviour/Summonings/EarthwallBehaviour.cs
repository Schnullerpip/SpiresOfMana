using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Finds the nearest object beneath the caster and instantiates a wall, coming out of the floor
/// </summary>
public class EarthwallBehaviour : A_SummoningBehaviour {

    public float initialDistanceToCaster = 2;

    public override void Execute(PlayerScript caster)
    {
        Vector3 aimDirection = GetAim(caster);

        GameObject wall = PoolRegistry.Instantiate(gameObject, caster.movement.mRigidbody.worldCenterOfMass + aimDirection * initialDistanceToCaster, Quaternion.LookRotation(aimDirection));
        wall.SetActive(true);
        NetworkServer.Spawn(wall);
    }

    protected override void ExecuteCollision_Host(Collision collision)
    {
        //should a collision with a wall really do anything?
    }
}