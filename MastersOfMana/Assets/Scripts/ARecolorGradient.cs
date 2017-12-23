using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component thats capable of recoloring various other components.
/// </summary>
public abstract class ARecolorGradient : ARecolor 
{
    public Gradient secondaryGradient;

    protected Gradient[] mOriginalGradients;

    public override void ChangeColor()
    {
        ChangeColorTo(secondaryGradient);
    }

    public override void ChangeColor(float t)
    {
        ChangeColorTo(secondaryGradient, t);
    }

    /// <summary>
    /// Changes the color to a custom color instantly.
    /// </summary>
    /// <param name="col">Col.</param>
    public abstract void ChangeColorTo(Gradient grad);

    /// <summary>
    /// Changes the color to a custom color via a linear interpolation.
    /// </summary>
    /// <param name="col">Col.</param>
    /// <param name="t">T.</param>
    public abstract void ChangeColorTo(Gradient grad, float t);
}