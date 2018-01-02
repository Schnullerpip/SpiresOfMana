using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSwing : MonoBehaviour 
{
    public NormalizedAnimationCurve x, y, z;

    private Vector3 mStartPos;

    private void Awake()
    {
        mStartPos = transform.localPosition;
    }

    private void Update () 
    {
        transform.localPosition = mStartPos + new Vector3(x.Evaluate(Time.time), y.Evaluate(Time.time), z.Evaluate(Time.time));
    }
}
