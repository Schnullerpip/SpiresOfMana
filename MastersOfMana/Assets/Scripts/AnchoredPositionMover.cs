using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AnchoredPositionMover : MonoBehaviour 
{
    public float speed = 10.0f;

    public AnimationCurve curve;

    private Vector2 mTargetPos;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Sets the position instantly and stops the transition.
    /// </summary>
    /// <param name="position">Position.</param>
    public void SetPosition(Vector2 position)
    {
        StopAllCoroutines();
        mTargetPos = position;
        rect.anchoredPosition = mTargetPos;
    }

    /// <summary>
    /// Sets the target, stops the current transition and begins to move to the new position.
    /// This will only happen if the target is a new one.
    /// </summary>
    /// <param name="position">Position.</param>
    public void SetTarget(Vector2 position)
    {
        if(position != mTargetPos)
        {
            mTargetPos = position;

            StopAllCoroutines();
            StartCoroutine(Move());
		}
    }

    private void OnDisable()
    {
        SetPosition(mTargetPos);
    }

    IEnumerator Move()
    {
        Vector2 startPos = rect.anchoredPosition;

        for (float f = 0; f <= 1; f += Time.deltaTime * speed)
        {
            rect.anchoredPosition = Vector2.LerpUnclamped(startPos, mTargetPos, curve.Evaluate(f));
            yield return null;
        }

        rect.anchoredPosition = mTargetPos;
    }
}