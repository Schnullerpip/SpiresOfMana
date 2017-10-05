using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelectionPanel : MonoBehaviour {

    public SpellRegistry spellregistry;
    public RectTransform spellList;
    public Button spellButtonPrefab;
    public Prototype.NetworkLobby.LobbyPlayer player;
    private bool initialized = false;

    private void OnEnable()
    {
        if (initialized)
            return;

        initialized = true;

        foreach(A_Spell spell in spellregistry.SpellList)
        {
            Button spellButton = GameObject.Instantiate(spellButtonPrefab);
            Image image = spellButton.transform.GetChild(0).GetComponent<Image>();
            if(image)
            {
                image.sprite = spell.icon;
            }
            spellButton.transform.SetParent(spellList);
            spellButton.onClick.AddListener(delegate { player.tradeSpells(spell); });
        }
    }
}
