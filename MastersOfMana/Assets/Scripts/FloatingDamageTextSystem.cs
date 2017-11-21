using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDamageTextSystem : MonoBehaviour {

    public FloatingDamageText FloatingText;
    public Canvas canvas;
    public PlayerScript player;
    public int numberOfPooledTexts = 25;
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
        transform.Translate(player.transform.position);
        transform.SetParent(player.transform,true);
    }

    public void CreateDamageText(int damage)
    {
        FloatingDamageText text = GetText();
        text.gameObject.SetActive(true);
        text.SetDamageText(damage);
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
