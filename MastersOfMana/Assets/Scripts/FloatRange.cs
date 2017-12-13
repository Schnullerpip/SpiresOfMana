using System;
using UnityEngine;

[System.Serializable]
/// <summary>
/// A data structure that holds a minimum and maximum float.
/// </summary>
public struct FloatRange  
{
	public float min;
	public float max;

	public FloatRange(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	/// <summary>
	/// Returns a random value between the minimum and maximum.
	/// </summary>
	public float Random()
	{
		return UnityEngine.Random.Range(min,max);
	}

	/// <summary>
	/// Returns a linearinterpolation between the minimum and maximum.
	/// </summary>
	/// <param name="t">T.</param>
	public float Lerp(float t)
	{
		return Mathf.Lerp(min, max, t);
	}
}