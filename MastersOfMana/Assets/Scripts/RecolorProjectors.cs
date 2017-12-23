using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolorProjectors : ARecolorSingleColor
{
    public Projector[] projectors;

    protected override void Init()
    {
        mOriginalColors = new Color[projectors.Length];

        for (int i = 0; i < projectors.Length; ++i)
        {
            //copy the material, since it works differently than renderers which to this implicitly
            projectors[i].material = new Material(projectors[i].material);
            mOriginalColors[i] = projectors[i].material.color;
        }
    }

    [ContextMenu("GetProjectionInChildren")]
    private void GetProjectorsInChildren()
    {
        projectors = GetComponentsInChildren<Projector>();
    }

    public override void ChangeColorTo(Color col)
    {
        for (int i = 0; i < projectors.Length; ++i)
        {
            projectors[i].material.color = col;
        }
    }

    public override void ChangeColorTo(Color col, float t)
    {
        for (int i = 0; i < projectors.Length; ++i)
        {
            projectors[i].material.color = Color.Lerp(projectors[i].material.color, col, t);
        }
    }

    public override void RevertColor()
    {
        for (int i = 0; i < projectors.Length; ++i)
        {
            projectors[i].material.color = mOriginalColors[i];
        }   
    }

    public override void RevertColor(float t)
    {
        for (int i = 0; i < projectors.Length; ++i)
        {
            projectors[i].material.color = Color.Lerp(projectors[i].material.color, mOriginalColors[i], t);
        }
    }
}
