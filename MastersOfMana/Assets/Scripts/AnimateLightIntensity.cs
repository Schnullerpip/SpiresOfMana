using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Light))]
public class AnimateLightIntensity : NetworkBehaviour
{
    public NormalizedAnimationCurve inCurve; 
    public NormalizedAnimationCurve outCurve;

    private Light mLight;

    private void Awake()
    {
        mLight = GetComponent<Light>();
    }

    [ClientRpc]
    public void RpcFade(bool fadeIn)
    {
        StopAllCoroutines();
        StartCoroutine(C_Fade(fadeIn ? inCurve : outCurve));
    }

    private IEnumerator C_Fade(NormalizedAnimationCurve curve)
    {
        float t = 0;

        while(t < 1)
        {
            mLight.intensity = curve.Evaluate(t);

            t += Time.deltaTime;
            yield return null;
        }

        mLight.intensity = curve.Evaluate(1);
    }
}
