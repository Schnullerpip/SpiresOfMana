using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class A_ServerMoveableSummoning : A_SummoningBehaviour, IServerMoveable {

    /// method to move the client, even though client has authority over his position
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    public void RpcAddForce(Vector3 force, int mode)
    {
        if (isLocalPlayer)
        {
            mRigid.AddForce(force, (ForceMode)mode);
        }
    }

    /// <summary>
    /// adds explosion force to player on server side - kinda
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    public void RpcAddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        if (isLocalPlayer)
        {
            mRigid.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        }
    }

    [ClientRpc]
    public void RpcStopMotion()
    {
        mRigid.velocity = Vector3.zero;
    }
}
