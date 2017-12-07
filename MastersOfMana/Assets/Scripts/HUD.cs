using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

    public List<GameObject> HudPrefabs = new List<GameObject>();
    private SpellHUD mSpellHUD;

    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
    }

	// Use this for initialization
	void Init()
    {
        foreach(GameObject obj in HudPrefabs)
        {
            Instantiate(obj, transform);
        }
        mSpellHUD = GetComponentInChildren<SpellHUD>();
    }

    public SpellHUD GetSpellHUD()
    {
        return mSpellHUD;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
    }
}
