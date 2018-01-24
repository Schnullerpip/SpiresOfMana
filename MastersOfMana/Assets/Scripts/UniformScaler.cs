using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformScaler : MonoBehaviour 
{
    public NormalizedAnimationCurve curve;
    public bool loop;

    public bool playOnEnable;

    public Mode mode = Mode.Relative;

    private Vector3 mStartScale;

    private void Awake()
    {
        mStartScale = transform.localScale;
    }

    private void OnEnable()
    {
        if(playOnEnable)
        {
            Play(!loop);
        }
    }

    [ContextMenu("Play Once")]
    public void PlayOnce()
    {
        Play(false);
    }

    /// <summary>
    /// Stops the animations loop, meaning it will finish it's current cycle.
    /// </summary>
    public void StopLoop()
    {
        loop = false;
    }

    public void Play(bool looping = true)
    {
		StopAllCoroutines ();
		loop = looping;
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine (Scaling ());
		}
    }

    private IEnumerator Scaling()
    {
        float timer = 0;
        float maxTime = 1 / curve.frequence;

        while (timer < maxTime)
        {
            transform.localScale = Size() + Vector3.one * curve.Evaluate(timer);

            timer += Time.deltaTime;
			yield return null;
			
			//this is just here to make sure the maxTime is correct if the frequency changed
			maxTime = 1 / curve.frequence;

            if (timer >= maxTime && loop)
            {
                timer -= maxTime;
            }
        }

        transform.localScale = Size() + Vector3.one * curve.Evaluate(maxTime);
    }

    public void Cancel()
    {
        transform.localScale = mStartScale;
    }

    private Vector3 Size()
    {
        return mode == Mode.Relative ? mStartScale : Vector3.zero;
    }

    public enum Mode
    {
        Relative, Absolute
    }
}
