using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFragForces : MonoBehaviour
{
    public Rigidbody[] rockFragments;
    public Transform[] rockDummies;

    public float force;

    void OnEnable()
    {
        for (int i = 0; i < rockFragments.Length; ++i)
        {
            var rockFrag = rockFragments[i];
            var rockDummy = rockDummies[i];

            rockFrag.transform.position = rockDummy.position;
            rockFrag.velocity = (Vector3.Normalize(rockDummy.position - transform.position)+Vector3.up)*force;
        }
    }
}