using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class reflectPlayer : MonoBehaviour {

    //the parent script, that will do all the work
    private EarthwallBehaviour ewb;

    private void Start()
    {
        ewb = GetComponentInParent<EarthwallBehaviour>();
    }

    void OnCollisionEnter(Collision collision)
    {
        ewb.CollisionRoutine(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        ewb.CollisionExitRoutine(collision);
    }
}