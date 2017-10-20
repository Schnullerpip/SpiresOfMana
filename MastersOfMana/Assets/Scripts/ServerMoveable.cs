using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerMoveable : NetworkBehaviour
{

    protected Rigidbody rigid;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public virtual void ServerAddForce_ForceMode_Acceleration(Vector3 force)
    {
        rigid.AddForce(force, ForceMode.Acceleration);
    }

    public virtual void ServerAddForce_ForceMode_Force(Vector3 force)
    {
        rigid.AddForce(force, ForceMode.Force);
    }

    public virtual void ServerAddForce_ForceMode_Impulse(Vector3 force)
    {
        rigid.AddForce(force, ForceMode.Impulse);
    }

    public virtual void ServerAddForce_ForceMode_VelocityCange(Vector3 force)
    {
        rigid.AddForce(force, ForceMode.VelocityChange);
    }
}

public class ServerMoveablePlayer :  ServerMoveable
{

    public override void ServerAddForce_ForceMode_Acceleration(Vector3 force)
    {
        RpcApplyServerForce(force, ForceMode.Acceleration);
    }

    public override void ServerAddForce_ForceMode_Force(Vector3 force)
    {
        RpcApplyServerForce(force, ForceMode.Force);
    }

    public override void ServerAddForce_ForceMode_Impulse(Vector3 force)
    {
        RpcApplyServerForce(force, ForceMode.Impulse);
    }

    public override void ServerAddForce_ForceMode_VelocityCange(Vector3 force)
    {
        RpcApplyServerForce(force, ForceMode.VelocityChange);
    }

    [ClientRpc]
    public void RpcApplyServerForce(Vector3 force, ForceMode mode)
    {
        rigid.AddForce(force, mode);
    }
}
