using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component thats capable of recoloring various other components.
/// </summary>
public abstract class ARecolor : MonoBehaviour 
{
    protected virtual void Awake()
    {
        Init();
    }

    protected abstract void Init();

    /// <summary>
    /// Changes the color to the specified secondary color instantly.
    /// </summary>
    public abstract void ChangeColor();

    /// <summary>
    /// Changes the color to the specified secondary color via a linear interpolation.
    /// </summary>
    /// <param name="t">T.</param>
    public abstract void ChangeColor(float t);

    /// <summary>
    /// Reverts the color back to the original color instantly.
    /// </summary>
    public abstract void RevertColor();

    /// <summary>
    /// Reverts the color back to the original color via a linear interpolation.
    /// </summary>
    /// <param name="t">T.</param>
	public abstract void RevertColor(float t);
}