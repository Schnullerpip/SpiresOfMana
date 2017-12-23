using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolorRenderer : Recolor
{
    public Renderer[] renderers;

    private void Awake()
    {
        mOriginalColors = new Color[renderers.Length];
        for (int i = 0; i < mOriginalColors.Length; ++i)
        {
            mOriginalColors[i] = renderers[i].material.color;
        }
    }

    [ContextMenu("GetRendererInChildren")]
    private void GetRendererInChildren()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public override void ChangeColor()
    {
        ChangeColorTo(secondaryColor);
    }

    public override void ChangeColor(float t)
    {
        ChangeColorTo(secondaryColor, t);
    }

    public override void RevertColor()
    {
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].material.color = mOriginalColors[i];
        }
    }

    public override void RevertColor(float t)
    {
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].material.color = Color.Lerp(renderers[i].material.color, mOriginalColors[i], t);
        }
    }

    public override void ChangeColorTo(Color col)
    {
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].material.color = col;
        }
    }

    public override void ChangeColorTo(Color col, float t)
    {
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].material.color = Color.Lerp(renderers[i].material.color, col, t);
        }
    }
}
