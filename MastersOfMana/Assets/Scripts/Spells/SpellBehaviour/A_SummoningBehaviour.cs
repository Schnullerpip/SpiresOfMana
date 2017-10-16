using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Subclass of A_SpellBehaviour, that defines spellbehaviours for things, that physically manifest, like fireballs
/// those things need to provide Remote Procedure Calls for de/activating themselves
/// </summary>
public abstract class A_SummoningBehaviour : A_SpellBehaviour {

    //the initial position of any spawned object
    public static readonly Vector3 OBLIVION = new Vector3(1000000, 1000000, 1000000);

    //important members
    protected Rigidbody mRigid;

    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            GetComponent<Collider>().enabled = false;
        }

        //summonings usually have rigid bodies - if so cache theirs in mRigid
        mRigid = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// invokes de/activation on all clients for this object
    /// </summary>
    /// <param name="activationState"></param>
    [ClientRpc]
    public void RpcSetActive(bool activationState)
    {
        Debug.Log("just activated a thing");
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

    /// <summary>
    /// positions the summoning far far away to overcome weird network interpolation issues
    /// when positioning the object far away, the snapping property of a networktransform will rather 'snap' the object into place, than interpolate its movement 
    /// </summary>
    [ClientRpc]
    protected void RpcPreventInterpolationIssues()
    {
        if (mRigid)
        {
            mRigid.Reset();
        }
        gameObject.transform.ResetTransformation();
        gameObject.transform.position = OBLIVION;
    }
}