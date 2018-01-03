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

    /// <summary>
    /// Initializes the Floating Damage Text System. Also repositions and reparents the object if a parent is provided.
    /// </summary>
    /// <returns>The init.</returns>
    /// <param name="parent">Parent.</param>
    public void Init(Transform parent)
    {
        Init();

        if (parent != null)
        {
            transform.Translate(parent.position);
        }
        transform.SetParent(parent, true);
    }

    /// <summary>
    /// Initializes the Floating Damage Text System.
    /// </summary>
    public void Init()
    {
        for(int i = 0; i < numberOfPooledTexts; i++)
        {
            FloatingDamageText text = Instantiate(FloatingText);
            text.gameObject.SetActive(false);
            text.transform.SetParent(canvas.transform, false);
            mTextPool.Add(text);
        }
        SubscribeToEvents();
    }

    public void SubscribeToEvents()
    {
        player.healthScript.OnDamageTaken += CreateDamageText;
        player.healthScript.OnHealTaken += CreateHealText;
    }

    public void UnsubscribeFromEvents()
    {
        player.healthScript.OnDamageTaken -= CreateDamageText;
        player.healthScript.OnHealTaken -= CreateHealText;
    }

    public void CreateDamageText(int damage)
    {
        CreateText(damage, damageColor);
    }

    public void CreateHealText(int heal)
    {
        CreateText(heal, healColor);
    }

    public void CreateText(int number, Color color)
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
