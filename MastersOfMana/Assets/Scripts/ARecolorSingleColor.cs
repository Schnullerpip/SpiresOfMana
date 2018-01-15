using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component thats capable of recoloring various other components.
/// </summary>
public abstract class ARecolorSingleColor : ARecolor 
{
    [ColorUsage(true,true,0,8,0.125f,3)]
    public Color secondaryColor = Color.gray;

    protected Color[] mOriginalColors;

    public override void ChangeColor()
    {
        ChangeColorTo(secondaryColor);
    }

    public override void ChangeColor(float t)
    {
        ChangeColorTo(secondaryColor, t);
    }

    /// <summary>
    /// Changes the color to a custom color instantly.
    /// </summary>
    /// <param name="col">Col.</param>
    public abstract void ChangeColorTo(Color col);

    /// <summary>
    /// Changes the color to a custom color via a linear interpolation.
    /// </summary>
    /// <param name="col">Col.</param>
    /// <param name="t">T.</param>
    public abstract void ChangeColorTo(Color col, float t);
}