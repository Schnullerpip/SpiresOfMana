using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class ServerMoveable : NetworkBehaviour
{

    //cached instance of the attached rigid body
    protected Rigidbody mRigidbody;

    /// <summary>
    /// Method that returns the player's velocity vector
    /// </summary>
    /// <returns></returns>
    public Vector3 GetVelocity()
    {
        return mRigidbody.velocity;
    }

    public virtual void Awake()
    {
        mRigidbody = gameObject.GetComponent<Rigidbody>();
    }


    /// method to move the client instance, even though client has authority over his position
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    public void RpcAddForce(Vector3 force, ForceMode mode)
    {
        mRigidbody.AddForce(force, mode);
    }

    /// <summary>
    /// adds explosion force to player on server side - kinda
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    public void RpcAddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        mRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
    }

    [ClientRpc]
    public void RpcAddForceAndUpdatePosition(Vector3 force, ForceMode mode, Vector3 newPosition)
    {
        mRigidbody.AddForce(force, mode);
        mRigidbody.position = newPosition;
    }

    [ClientRpc]
    public void RpcStopMotion()
    {
        mRigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// allows the server and thus the spells, to affect the players position
    /// </summary>
    /// <param name="vec3"></param>
    [ClientRpc]
    public void RpcSetPosition(Vector3 vec3)
    {
        mRigidbody.position = vec3;
    }
}