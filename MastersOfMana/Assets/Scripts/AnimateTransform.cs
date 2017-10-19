using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimateTransform : MonoBehaviour {

	[Tooltip("Make sure that the values are all normalized")]
	public AnimationCurve animationCurve;
	public float lifeTime = 1;
	public float amplitude = 4;

	public Vector3 axisInfluence = Vector3.one;

	public UnityEvent onAnimationFinished;

	private float mTime = 0;

	void OnEnable()
	{
		mTime = 0;
	}

	void Update()
	{
		if(mTime <= lifeTime)
		{
			transform.localScale = axisInfluence * animationCurve.Evaluate(mTime / lifeTime) * amplitude;
			mTime += Time.deltaTime;
		}
		else
		{
			onAnimationFinished.Invoke();
		}
	}
}
