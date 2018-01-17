using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManipulation : MonoBehaviour
{

    public AnimationCurve Curve;
    private float mTimeCount;
    private Vector3 mCachedScale;

    void Awake()
    {
        mCachedScale = transform.localScale;
    }

    void OnEnable()
    {
        mTimeCount = 0;
        Scale();
    }

	// Update is called once per frame
	void Update ()
	{
	    mTimeCount += Time.deltaTime;

	    Scale();

	}

    private void Scale()
    {
	    Vector3 scale = mCachedScale;
        scale.x *= Curve.Evaluate(mTimeCount);
        scale.y *= Curve.Evaluate(mTimeCount);
        scale.z *= Curve.Evaluate(mTimeCount);
	    transform.localScale = scale;
    }
}
