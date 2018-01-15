using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlatformSpiresEffect : NetworkBehaviour
{
    public Renderer[] renderers;

    public FloatRange emission = new FloatRange(0, 10);
    public float speed = 1;

    private Material mMaterial;
    private int mEmissionPropID;
    private int mColorPropID;

    void Awake()
    {
        mMaterial = new Material(renderers[0].material);
        mEmissionPropID = Shader.PropertyToID("_EmissionStrength");
        mColorPropID = Shader.PropertyToID("_EmissionColor");

        mMaterial.SetFloat(mEmissionPropID, 0);

        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].material = mMaterial;
        }
    }

    [ClientRpc]
    public void RpcActivate(Color color)
    {
        mMaterial.SetColor(mColorPropID, color);
        StopAllCoroutines();
        StartCoroutine(EmissionRamp(Direction.INCREASE));
    }

    [ClientRpc]
    public void RpcDeactivate()
    {
        StopAllCoroutines();
        StartCoroutine(EmissionRamp(Direction.DECREASE));
    }

    private float mInterpolator = 0;

    private IEnumerator EmissionRamp(Direction direction)
    {
        float target = direction == Direction.INCREASE ? 1 : 0;

        while (Mathf.Abs(target - mInterpolator) > float.Epsilon)
        {
            mInterpolator = Mathf.MoveTowards(mInterpolator, target, speed * Time.deltaTime);

            mMaterial.SetFloat(mEmissionPropID, emission.Lerp(mInterpolator));
            yield return null;
        }
    }

    internal enum Direction
    {
        INCREASE = 1, DECREASE = -1
    }
}
