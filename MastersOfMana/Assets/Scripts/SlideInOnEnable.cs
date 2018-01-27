using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SlideInOnEnable : MonoBehaviour
{
    public Vector3 initialOffset;
    public float speed = 1;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public float delay = 0;

    private Vector3 mInitPos;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        mInitPos = rect.anchoredPosition3D;
    }

    private void OnEnable()
    {
        rect.anchoredPosition3D = initialOffset;
        StartCoroutine(Move());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        rect.anchoredPosition3D = mInitPos;
    }

    private IEnumerator Move()
    {
        for (float t = - delay; t < 1; t += Time.deltaTime * speed)
        {
            if(t >= 0)
            {
				rect.anchoredPosition3D = Vector3.LerpUnclamped(mInitPos + initialOffset, mInitPos, curve.Evaluate(t));
            }
            yield return null;
        }

        rect.anchoredPosition3D = mInitPos;
    }
}