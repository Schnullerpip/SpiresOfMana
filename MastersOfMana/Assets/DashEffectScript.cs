using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class DashEffectScript : NetworkBehaviour {

    [SyncVar]
    public Vector3 from;
    [SyncVar]
    public Vector3 to;

    public float Lifetime;

    public float Speed;


    private void FixedUpdate()
    {
        //continuously move the effect so that the particle trail will emit trailparticles
        transform.position = Vector3.MoveTowards(transform.position, to, Speed*Time.deltaTime);
    }
}