using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextAppearingFX : MonoBehaviour 
{
	public float charactersPerSecond = 300;

	private const string BEGIN_COL_TAG = "<color=#ffffff00>";
    private const string END_COL_TAG = "</color>";

    private Text mText;
    private string mWholeString;

    private void Awake()
    {
        mText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        SetText(mText.text);
    }

    private void OnDisable()
    {
        mText.text = mWholeString;
    }

    /// <summary>
    /// Sets the text and starts the appearance effect.
    /// </summary>
    /// <param name="text">Text.</param>
    public void SetText(string text)
    {
        mWholeString = text;
        StopAllCoroutines();
        StartCoroutine(Run());
    }

	IEnumerator Run() 
    {
        float progress = 0;
        int charIndex = 0;

        while(charIndex <= mWholeString.Length)
        {
            //we use the <color> tag to color in the second part of the text. This is used in order to ensure that the linebreaks are already correct and the text doesn't pop when the line is full
            mText.text = string.Concat(mWholeString.Substring(0, charIndex), BEGIN_COL_TAG, mWholeString.Substring(charIndex, mWholeString.Length - charIndex), END_COL_TAG);

            //a seperate float is used to enable a framerate independent effect
            progress += Time.deltaTime * charactersPerSecond;
            //truncate the progress
            charIndex = (int)progress;
            yield return null;
		}

        mText.text = mWholeString;
	}
}
