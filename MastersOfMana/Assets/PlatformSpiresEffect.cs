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

    //Events
    public delegate void LoadingZoneWillSpawn();
    public delegate void LoadingZoneDisappeared();
    /// <summary>
    /// everyone who wants to be informed, whenever a loading zone will spawn soon, should register on this event
    /// </summary>
    public event LoadingZoneWillSpawn OnLoadingZoneWillSpawn;
    /// <summary>
    /// everyone who wants to be informed, whenever a loading zone unspawns, should register on this event
    /// </summary>
    public event LoadingZoneDisappeared OnLoadingZoneDisappeared;

    void Awake()
    {
        mMaterial = new Material(renderers[0].materials[1]);
        mEmissionPropID = Shader.PropertyToID("_EmissionStrength");
        mColorPropID = Shader.PropertyToID("_EmissionColor");

        mMaterial.SetFloat(mEmissionPropID, 0);

        //since a material in the array can not be set (for whatever reason), we have to cache the array
        Material[] matArray = renderers[0].materials;
        matArray[1] = mMaterial;

        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].materials = matArray;
        }
    }

    [ClientRpc]
    public void RpcActivate(Color color)
    {
        mMaterial.SetColor(mColorPropID, color);
        StopAllCoroutines();
        StartCoroutine(EmissionRamp(Direction.INCREASE));

        //inform the listeners, that a loading zone will spawn soon (announcementTime)
        if (OnLoadingZoneWillSpawn != null)
        {
            OnLoadingZoneWillSpawn();
        }
    }

    [ClientRpc]
    public void RpcDeactivate()
    {
        StopAllCoroutines();
        StartCoroutine(EmissionRamp(Direction.DECREASE));

        if (OnLoadingZoneDisappeared != null)
        {
            OnLoadingZoneDisappeared();
        }
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
