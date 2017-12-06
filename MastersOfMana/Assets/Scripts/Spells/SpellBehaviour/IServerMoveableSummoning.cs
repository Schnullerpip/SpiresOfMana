using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ServerMoveable))]
public abstract class A_ServerMoveableSummoning : A_SummoningBehaviour
{
    public ServerMoveable serverMoveable;

    public override void Awake()
    {
        base.Awake();

        serverMoveable = GetComponent<ServerMoveable>();
        if (!serverMoveable)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException("no ServerMoveable script attached!");
        }
    }
}
