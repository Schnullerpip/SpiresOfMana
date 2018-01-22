using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsIndicator : MonoBehaviour {
	private int mIntensityLevel;
	private float mIntensityFloat;

	public Slider statslider;
	//public float sliderSpeed;
	[Range(0,0.1f)]
	public float sliderSpeed;
	public Gradient colorScale;

	private FloatRange sliderScale = new FloatRange(0,3);

	public void SetIntensityLevel(int level)
	{
		mIntensityLevel = level;
		if (level == 0)
		{
			mIntensityFloat = 0;
		} 
		else
		{
			mIntensityFloat = sliderScale.InverseLerp (level);
		}
	}

	public int GetIntensityLevel()
	{
		return mIntensityLevel;
	}

	void Update()
	{
		float difference = statslider.value - mIntensityFloat;
		if (Mathf.Abs (difference) <= sliderSpeed) 
		{
			statslider.value = mIntensityFloat;
		} 
		else 
		{
			if (difference > 0) 
			{
				statslider.value -= sliderSpeed;
			} 
			else if (difference <= 0) 
			{
				statslider.value += sliderSpeed;
			}
		}

		statslider.fillRect.GetComponentInChildren<Image> ().color = colorScale.Evaluate (statslider.value);

	}
}
