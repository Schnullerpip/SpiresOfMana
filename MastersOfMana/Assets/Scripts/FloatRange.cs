using System;
using UnityEngine;

[System.Serializable]
public struct FloatRange  
{
	public float min;
	public float max;

	public FloatRange(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	public float Random()
	{
		return UnityEngine.Random.Range(min,max);
	}
}