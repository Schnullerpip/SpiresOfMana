using UnityEngine;
using System.Collections;

public class UISpellButton : MonoBehaviour
{
    public A_Spell spell;
    public bool isUltimate = false;

    public bool isInSpellSlot = false;

    public UnityEngine.UI.Image icon;

    public Color chosenColor;

    public float colorTransitionSpeed = 8.0f;

    private Color mColor;

    private void Awake()
    {
        mColor = icon.color;
    }

    private void OnDisable()
    {
        icon.color = mColor;
    }

    public void SetColor(Color color)
    {
        StopAllCoroutines();
        if(gameObject.activeInHierarchy)
        {
			StartCoroutine(TransitionColor(color));
        }
        else
        {
            icon.color = color;
        }
    }

    public void SetIsChosen(bool value)
    {
        isInSpellSlot = value;

        SetColor(isInSpellSlot ? chosenColor : mColor);
    }

    private IEnumerator TransitionColor(Color toColor)
    {
        Color startColor = icon.color;

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime * colorTransitionSpeed)
        {
            icon.color = Color.Lerp(startColor, toColor, t);
			yield return null;
        }

        icon.color = toColor;
    }
}
