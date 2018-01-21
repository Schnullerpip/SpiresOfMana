using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageColor : MonoBehaviour {

    public Color color = Color.gray;

    public float speed = 1;

    private Image mImage;
    private Color mInitColor;

    private void Awake()
    {
        mImage = GetComponent<Image>();
        mInitColor = mImage.color;
    }

    public void Change()
    {
        Change(color);
    }

    public void Change(Color col)
    {
        StopAllCoroutines();
        StartCoroutine(Transition(col));
    }

    public void Revert()
    {
        if(gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(Transition(mInitColor));
		}
        else
        {
            mImage.color = mInitColor;
        }
    }

    IEnumerator Transition(Color toColor)
    {
        for (float f = 0; f < 1; f += Time.deltaTime * speed)
        {
            mImage.color = Color.Lerp(mImage.color, toColor, f);
			yield return null;
        }

        mImage.color = toColor;
    }

    public void Cancel()
    {
        StopAllCoroutines();
        mImage.color = mInitColor;
    }
}
