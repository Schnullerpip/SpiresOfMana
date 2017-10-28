using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IServerMoveable
{
    /// method to move the client, even though client has authority over his position
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    void RpcAddForce(Vector3 force, int mode);

    /// <summary>
    /// adds explosion force to player on server side - kinda
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    void RpcAddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius);

    [ClientRpc]
    void RpcStopMotion();
}
