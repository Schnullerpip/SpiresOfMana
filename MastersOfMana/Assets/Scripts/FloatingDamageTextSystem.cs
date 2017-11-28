using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDamageTextSystem : MonoBehaviour {

    public FloatingDamageText FloatingText;
    public Canvas canvas;
    public PlayerScript player;
    public int numberOfPooledTexts = 25;
    public Color damageColor;
    public Color healColor;
    private int mCurrentObj = 0;

    private List<FloatingDamageText> mTextPool = new List<FloatingDamageText>();

    public void Init()
    {
        for(int i = 0; i < numberOfPooledTexts; i++)
        {
            FloatingDamageText text = Instantiate(FloatingText);
            text.gameObject.SetActive(false);
            text.transform.SetParent(canvas.transform, false);
            mTextPool.Add(text);
        }

        player.healthScript.OnDamageTaken += CreateDamageText;
        player.healthScript.OnHealTaken += CreateHealText;
        transform.Translate(player.transform.position);
        transform.SetParent(player.transform,true);
    }

    public void CreateDamageText(int damage)
    {
        createText(damage, damageColor);
    }

    public void CreateHealText(int heal)
    {
        createText(heal, healColor);
    }

    public void createText(int number, Color color)
    {
        FloatingDamageText text = GetText();
        text.gameObject.SetActive(true);
        text.SetDamageText(number);
        text.text.color = color;
    }

    private FloatingDamageText GetText()
    {
        FloatingDamageText obj = mTextPool[mCurrentObj];
        mCurrentObj++;
        if(mCurrentObj == mTextPool.Count)
        {
            mCurrentObj = 0;
        }
        return obj;
    }
}
