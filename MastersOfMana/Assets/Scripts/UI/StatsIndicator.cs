using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatsIndicator : MonoBehaviour 
{
	public Slider statslider;

	[Range(0,10)]
	public float sliderSpeed;
	public Gradient colorScale;

    private int mIntensityLevel;
    private float mIntensityFloat;

    private Image mFill;
	private FloatRange sliderScale = new FloatRange(0,3);

    private bool mDraggingSlider = false;

    private void Awake()
    {
        mFill = statslider.fillRect.GetComponentInChildren<Image>();

        AttachEventTrigger();
    }

    private void AttachEventTrigger()
    {
        //we add these triggers to keep track wether or not the user is manipulating the sliders for fun
        
        EventTrigger trigger = statslider.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((eventData) => { mDraggingSlider = true; });
        trigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((eventData) => { mDraggingSlider = false; });
        trigger.triggers.Add(pointerUpEntry);
    }

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
        //if the slider is manipulated, we stop adjusting the value so the slider stays when clicked but not moved
        //once released, the value is changed by code to represent the correct value
        if(!mDraggingSlider)
        {
			//would move the value linearly
			//statslider.value = Mathf.MoveTowards(statslider.value, mIntensityFloat, Time.deltaTime * sliderSpeed);         

            //technically not the desired use of a lerp, but it makes a nice dampening curve
			statslider.value = Mathf.Lerp(statslider.value, mIntensityFloat, Time.deltaTime * sliderSpeed);         
        }

        //float difference = statslider.value - mIntensityFloat;
		//if (Mathf.Abs (difference) <= sliderSpeed) 
		//{
		//	statslider.value = mIntensityFloat;
		//} 
		//else 
		//{
		//	if (difference > 0) 
		//	{
		//		statslider.value -= sliderSpeed;
		//	} 
		//	else if (difference <= 0) 
		//	{
		//		statslider.value += sliderSpeed;
		//	}
		//}

		mFill.color = colorScale.Evaluate (statslider.value);
	}
}
