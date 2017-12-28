using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NormalizedAnimationCurve
{
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,1));
    public float amplitude = 1;
    public float frequence = 1;
    public float offset = 0;

    public float Evaluate(float t)
    {
        return curve.Evaluate(t * frequence + offset) * amplitude;
    }
}
