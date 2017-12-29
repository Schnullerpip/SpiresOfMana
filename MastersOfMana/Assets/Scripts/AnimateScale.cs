using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateScale : MonoBehaviour
{
    public AnimationCurve curve;

    private Vector3 initalScale;
    private float t = 0;

    private void Awake()
    {
        initalScale = transform.localScale;
    }

    private void OnEnable()
    {
        t = 0;
    }

    private void Update()
    {
		transform.localScale = initalScale * curve.Evaluate(t);
        t += Time.deltaTime;
    }
}
