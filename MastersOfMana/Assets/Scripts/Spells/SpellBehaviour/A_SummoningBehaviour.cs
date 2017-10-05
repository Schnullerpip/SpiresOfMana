using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class A_SummoningBehaviour : A_SpellBehaviour {

    [ClientRpc]
    public void RpcSetActive(bool activationState)
    {
        gameObject.SetActive(activationState);
    }

    /// <summary>
    /// makes sure collisionhandling is executed exclusively on the server, including every operation, that comes with it, like lifedrain etc.
    /// each A_SummoningBehaviour will have to implement an ExecuteCollisionOnServer method, that will be called in the event of a collision
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
        {
            return;
        }
        ExecuteCollisionOnServer(collision);
    }

    /// <summary>
    /// actually handles the collision, but should only ever be invoked from the OnCollision method of the A_SummoningBehaviour super class
    /// </summary>
    /// <param name="collision"></param>
    protected abstract void ExecuteCollisionOnServer(Collision collision);
}
