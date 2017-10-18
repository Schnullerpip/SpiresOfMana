using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellHUD : MonoBehaviour
{

    public RectTransform
        SpellSlot1,
        SpellSlot2,
        SpellSlot3;

    private PlayerScript localPlayer;

	// Use this for initialization
	void OnEnable ()
    {
        GameManager.OnGameStarted += Init;
	}

    public void Init()
    {
        //Fill SpellSlots with correct spellIcons
        localPlayer = GameManager.instance.localPlayer;
        SpellSlot1.GetComponentInChildren<Image>().sprite = localPlayer.spellSlot_1.spell.icon;
        SpellSlot2.GetComponentInChildren<Image>().sprite = localPlayer.spellSlot_2.spell.icon;
        SpellSlot3.GetComponentInChildren<Image>().sprite = localPlayer.spellSlot_3.spell.icon;
    }
}
