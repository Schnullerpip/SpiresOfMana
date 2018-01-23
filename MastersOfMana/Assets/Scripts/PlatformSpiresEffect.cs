using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlatformSpiresEffect : NetworkBehaviour
{
    public Renderer[] renderers;
    public Renderer platformRenderer;

    public FloatRange emission = new FloatRange(0, 10);
    public float speed = 1;

    private MaterialInstance mSpireMat;     private MaterialInstance mPlatformMat; 
    //Events
	public delegate void LoadingZoneWillSpawn(Color color);
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
        mSpireMat = new MaterialInstance();
        mPlatformMat = new MaterialInstance();

        //spire materials
        mSpireMat.Init(renderers[0].materials[1]);         mSpireMat.SetEmission(0);

        //since a material in the array can not be set (for whatever reason), we have to cache the array
        Material[] matArray = renderers[0].materials;
        matArray[1] = mSpireMat.material;

        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].materials = matArray;
        }

        //platform material
        mPlatformMat.Init(platformRenderer.material);
        mPlatformMat.SetEmission(0);

        platformRenderer.material = mPlatformMat.material;
    }

    [ClientRpc]
    public void RpcActivate(Color color)
    {
        mSpireMat.SetColor(color);
        mPlatformMat.SetColor(color);

        StopAllCoroutines();
        StartCoroutine(EmissionRamp(Direction.INCREASE));

        //inform the listeners, that a loading zone will spawn soon (announcementTime)
        if (OnLoadingZoneWillSpawn != null)
        {
            OnLoadingZoneWillSpawn(color);
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

            float em = emission.Lerp(mInterpolator);

            mSpireMat.SetEmission(em);
            mPlatformMat.SetEmission(em);
            yield return null;
        }
    }

    internal enum Direction
    {
        INCREASE = 1, DECREASE = -1
    }

    internal struct MaterialInstance     {         internal Material material;         private int emissionPropertyID;         private int colorPropertyID;          internal void Init(Material original)         {             material = new Material(original);             emissionPropertyID = Shader.PropertyToID("_EmissionStrength");             colorPropertyID = Shader.PropertyToID("_EmissionColor");         }          internal void SetEmission(float value)         {             material.SetFloat(emissionPropertyID, value);         }          internal void SetColor(Color value)         {
            material.SetColor(colorPropertyID, value);
        }     }
}
