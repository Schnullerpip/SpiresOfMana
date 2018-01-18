using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManipulation : MonoBehaviour
{

    public AnimationCurve Curve;
    private float mTimeCount;
    private Vector3 mCachedScale;

    [Tooltip("Will add a random number to the evaluation of the scalemanipulation, making it look a bit more uniquely when next to many other objects, that do the same")]
    public bool IncludeRandomFactor;
    private float mRandomFactor;

    void Awake()
    {
        mCachedScale = transform.localScale;
    }

    void OnEnable()
    {
        mTimeCount = 0;
        Scale();

        if (IncludeRandomFactor)
        {
            mRandomFactor = Mathf.Clamp(Random.Range(-0.3f, 0.8f), 0.0f, 0.8f);
        }
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
        var evaluationX = mTimeCount + (IncludeRandomFactor ? mRandomFactor : 0.0f);
        scale.x *= Curve.Evaluate(evaluationX);
        scale.y *= Curve.Evaluate(evaluationX);
        scale.z *= Curve.Evaluate(evaluationX);
	    transform.localScale = scale;
    }
}
