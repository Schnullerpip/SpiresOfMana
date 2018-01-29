using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour {

	public Text text;
	public float dotDelay = 1.0f;
	public List<string> loadingTexts;
	private int counter = 0;

	void OnEnable()
	{
		StartCoroutine (LoadingDots(dotDelay));
	}

	IEnumerator LoadingDots(float seconds)
	{
		text.text = loadingTexts[counter];
		counter++;
		if (counter == loadingTexts.Count) 
		{
			counter = 0;
		}
		yield return new WaitForSeconds(seconds);
		StartCoroutine (LoadingDots(seconds));
	}

	void OnDisable()
	{
		StopAllCoroutines ();
	}
}
