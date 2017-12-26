using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class ServerMoveable : NetworkBehaviour
{

    //cached instance of the attached rigid body
    public Rigidbody mRigidbody;

    //determines whether the instance allows movement or not
    [SyncVar] private bool mMovementAllowed = true;

    [ClientRpc]
    public void RpcSetMovementAllowed(bool allowance)
    {
        mMovementAllowed = allowance;
    }
    public bool GetMovementAllowed()
    {
        return mMovementAllowed;
    }

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

    ///<summary>
    /// method to move the client instance, even though client has authority over his position
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    public void RpcAddForce(Vector3 force, ForceMode mode)
    {
        if (!mMovementAllowed)
        {
            return;
        }
        mRigidbody.AddForce(force, mode);
    }

	[ClientRpc]
	public void RpcSetVelocity(Vector3 velocity)
	{
        if (!mMovementAllowed)
        {
            return;
        } 
		mRigidbody.velocity = velocity;
	}

    [ClientRpc]
    public void RpcSetVelocityY(float veloY)
    {
        if (!mMovementAllowed)
        {
            return;
        } 
        mRigidbody.SetVelocityY(veloY);
    }

    [ClientRpc]
    public void RpcAddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        if (!mMovementAllowed)
        {
            return;
        } 
        mRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
    }

    [ClientRpc]
    public void RpcAddForceAndUpdatePosition(Vector3 force, ForceMode mode, Vector3 newPosition)
    {
        if (!mMovementAllowed)
        {
            return;
        } 
        mRigidbody.AddForce(force, mode);
        mRigidbody.position = newPosition;
    }

    [ClientRpc]
    public void RpcSetVelocityAndMovePosition(Vector3 velocity, Vector3 newPosition)
    {
        if (!mMovementAllowed)
        {
            return;
        } 
        mRigidbody.velocity = velocity;
        mRigidbody.MovePosition(newPosition);
    }

    [ClientRpc]
    public void RpcStopMotion()
    {
        if (!mMovementAllowed)
        {
            return;
        } 
        mRigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// allows the server and thus the spells, to affect the players position
    /// </summary>
    /// <param name="vec3"></param>
    [ClientRpc]
    public void RpcSetPosition(Vector3 vec3)
    {
        if (!mMovementAllowed)
        {
            return;
        } 
        mRigidbody.position = vec3;
    }

    [ClientRpc]
    public void RpcMovePosition(Vector3 vec3)
    {
        if (!mMovementAllowed)
        {
            return;
        } 
        mRigidbody.MovePosition(vec3);
    }
}