using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Finds the nearest object beneath the caster and instantiates a wall, coming out of the floor
/// </summary>
public class EarthwallBehaviour : A_SummoningBehaviour {

    public float initialDistanceToCaster = 2;

    public override void Preview(PlayerScript caster)
    {
        base.Preview(caster);

        preview.instance.MoveAndRotate(caster.movement.mRigidbody.worldCenterOfMass + GetAimLocal(caster) * initialDistanceToCaster, Quaternion.LookRotation(GetAimLocal(caster)));
    }

    public override void StopPreview(PlayerScript caster)
    {
        base.StopPreview(caster);
        preview.instance.Deactivate();
    }

    public override void Execute(PlayerScript caster)
    {
        GameObject wall = PoolRegistry.Instantiate(gameObject, caster.movement.mRigidbody.worldCenterOfMass + GetAim(caster) * initialDistanceToCaster, Quaternion.LookRotation(GetAim(caster)));
        wall.SetActive(true);
        NetworkServer.Spawn(wall);
    }

    protected override void ExecuteCollision_Host(Collision collision)
    {
        //should a collision with a wall really do anything?
    }
}