using UnityEngine;

public class UISpellButton : MonoBehaviour
{
    public A_Spell spell;
    public bool isUltimate = false;

    public bool isInSpellSlot = false;

    public UnityEngine.UI.Image icon;

    public Color chosenColor;
    private Color mColor;

    private void Awake()
    {
        mColor = icon.color;
    }

    public void SetColor(Color color)
    {
        icon.color = color;
    }

    public void SetIsChosen(bool value)
    {
        isInSpellSlot = value;
        icon.color = isInSpellSlot ? chosenColor : mColor;
    }
}
