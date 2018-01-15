using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateUVOffset : MonoBehaviour 
{
    public Material material;
    public string propertyName = "_MainTex";

	public NormalizedAnimationCurve xCurve;
    public NormalizedAnimationCurve yCurve;

    private Vector2 mUV = new Vector2(0,0);

    private int mPropertyId;
    private Vector3 mStartOffset;

    private void Awake()
    {
        mPropertyId = Shader.PropertyToID(propertyName);
        mStartOffset = material.GetTextureOffset(mPropertyId);
    }

    void Update () 
    {
        mUV.x = xCurve.Evaluate(Time.time);
        mUV.y = yCurve.Evaluate(Time.time);

        material.SetTextureOffset(mPropertyId, mUV);
	}

    private void OnDestroy()
    {
        Revert();
    }

    private void OnDisable()
    {
        Revert();
    }

    void Revert()
    {
        material.SetTextureOffset(mPropertyId, mStartOffset);
    }
}
