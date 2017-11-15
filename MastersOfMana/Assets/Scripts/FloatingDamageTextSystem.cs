using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDamageTextSystem : MonoBehaviour {

    public GameObject FloatingText;
    public int numberOfPooledTexts = 25;

    private List<GameObject> mTextPool = new List<GameObject>();
    // Use this for initialization
    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
    }

    public void Init()
    {
        for(int i = 0; i < numberOfPooledTexts; i++)
        {
            GameObject text = Instantiate(FloatingText);
            text.SetActive(false);
            mTextPool.Add(text);
        }
    }

    public void CreateDamageText(int damage, Transform position)
    {

    }
}
