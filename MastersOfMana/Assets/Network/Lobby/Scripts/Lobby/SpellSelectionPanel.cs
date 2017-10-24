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
    private Prototype.NetworkLobby.LobbyManager.BackButtonDelegate oldBackDelegate;

    private void OnEnable()
    {
        if (initialized)
            return;

        initialized = true;

        for(int i = 0; i < spellregistry.SpellList.Count; i++)
        {
            A_Spell spell = spellregistry.SpellList[i];
            Button spellButton = GameObject.Instantiate(spellButtonPrefab);
            Image image = spellButton.transform.GetChild(0).GetComponent<Image>();
            if(image)
            {
                image.sprite = spell.icon;
            }
            spellButton.transform.SetParent(spellList);
            spellButton.transform.localScale = new Vector3(1, 1, 1);
            spellButton.onClick.AddListener(delegate { player.tradeSpells(spell); });
        }

        oldBackDelegate = Prototype.NetworkLobby.LobbyManager.s_Singleton.backDelegate;
        Prototype.NetworkLobby.LobbyManager.s_Singleton.backDelegate = delegate ()
        {
            gameObject.SetActive(false);
        };
    }

    private void OnDisable()
    {
        Prototype.NetworkLobby.LobbyManager.s_Singleton.backDelegate = oldBackDelegate;
    }
}
