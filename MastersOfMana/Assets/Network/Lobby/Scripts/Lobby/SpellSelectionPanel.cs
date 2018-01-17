using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellSelectionPanel : MonoBehaviour {

    public SpellRegistry spellregistry;
    public RectTransform normalSpellList;
    public RectTransform ultimateSpellList;
    public UISpellButton spellButtonPrefab;
    public SpellDescription spellDescription;
    public ReadyButton readyButton;
    private PlayerSpells mPlayerSpells;
    private List<A_Spell> mPlayerSpellList = new List<A_Spell>();
    private HUD mHUD;
    private List<Button> spellButtons = new List<Button>();
    private Dictionary<A_Spell, Button> mSpellButtonDictionary = new Dictionary<A_Spell, Button>();

    public SpellAssignmentPopup popupMenu;
    public PopupMenu unableToAssignPopup;
    public RectTransform[] selectionHighlights;

    private Rewired.Player playerInput;
    private PlayerLobby lobbyPlayer;

    private void Start()
    {
        playerInput = GameManager.instance.localPlayer.GetRewired();
        mPlayerSpells = GameManager.instance.localPlayer.GetPlayerSpells();
        lobbyPlayer = GameManager.instance.localPlayer.playerLobby;
        mPlayerSpellList = GetPlayerSpellList();
        mHUD = GetComponentInParent<HUD>();
        FillContainer(normalSpellList, spellregistry.spellList);
        FillContainer(ultimateSpellList, spellregistry.ultimateSpellList);
        ValidateSpellSelection();

        //display the player first spell
        spellDescription.SetDescription(mPlayerSpells.spellslot[0].spell.spellDescription);
    }

    private void FillContainer(RectTransform container, List<A_Spell> spells)
    {
        for (int i = 0; i < spells.Count; i++)
        {
            A_Spell spell = spells[i];
            UISpellButton spellButtonScript = GameObject.Instantiate(spellButtonPrefab);
            Button spellButton = spellButtonScript.gameObject.GetComponent<Button>();
			spellButton.name += " " + spell.name;
            if(spells[i].spellID >= 100) //This relies on the fact, that all ultimates get an ID assigned by the spellregistry that's higher than 100!
            {
                spellButtonScript.isUltimate = true;
                spellButton.onClick.AddListener(()=>OnHoverClick(3, true));
            }
            spellButtonScript.spell = spells[i];
            Image image = spellButton.transform.GetChild(1).GetComponent<Image>();
            if (image)
            {
                image.sprite = spell.icon;
            }
            spellButton.transform.SetParent(container);
            spellButton.transform.localScale = Vector3.one;

            EventTrigger trigger = spellButton.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener((eventData) => { OnPointerEnterSpellButton(spellButtonScript); });
            trigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((eventData) => { OnPointerExitSpellButton(spellButtonScript); });
            trigger.triggers.Add(pointerExitEntry);

            spellButtons.Add(spellButton);
            mSpellButtonDictionary.Add(spell, spellButton);
        }
    }

    private List<A_Spell> GetPlayerSpellList()
    {
        List<A_Spell> playerSpells = new List<A_Spell>();
        foreach(PlayerSpells.SpellSlot slot in mPlayerSpells.spellslot)
        {
            playerSpells.Add(slot.spell);
        }
        return playerSpells;
    }

	//var derMaschine = ((Rewired.Integration.UnityUI.RewiredStandaloneInputModule)eventData.currentInputModule).pointerEventDataOnClick.pointerEnter;

    private void OnUltiSpellButton(A_Spell spell)
    {
        TradeSpells(spell, 3);
    }

    private void OnPointerEnterSpellButton(UISpellButton spellButton)
    {
        spellDescription.SetDescription(spellButton.spell.spellDescription);
        if(!GameManager.instance.localPlayer.playerLobby.isReady)
        {
            popupMenu.Open(spellButton.isUltimate);
        }
    }

    private void OnPointerExitSpellButton(UISpellButton spellButton)
    {
        popupMenu.Close();
    }

    /// <summary>
    /// Checks if chosen spell has already been selected and trades spellslot
    /// </summary>
    /// <param name="spell"></param>
    /// <returns>
    /// Returns Spell that has been traded with. Returns null if no spell was traded
    /// </returns>
    public A_Spell TradeSpells(A_Spell spell, int inputIndex)
    {
        A_Spell tradedSpell = null;
        for (int i = 0; i < mPlayerSpellList.Count-1; i++)
        {
            //TODO: Replcae mPlayerSpells.currentSpell with own index
            //Check if the chosen spell is already used
            if (mPlayerSpellList[i] == spell && i != inputIndex)
            {
                //If the spell is already used, just swap the spell to change with the old position of the new spell
                tradedSpell = mPlayerSpellList[inputIndex];
                mPlayerSpellList[i] = tradedSpell;
                break;
            }
        }
        mPlayerSpellList[inputIndex] = spell;
        ValidateSpellSelection();
        return tradedSpell;
    }

    private void ValidateSpellSelection()
    {
        //seperate illegal and legal spells
        List<int> illegalSpellSlots = new List<int>();
        List<A_Spell> legalSpells = new List<A_Spell>();
        for (int i = 0; i < mPlayerSpellList.Count - 1; i++)
        {
            if (!spellregistry.ValidateNormal(mPlayerSpellList[i]))
            {
                illegalSpellSlots.Add(i);
            }
            else
            {
                legalSpells.Add(mPlayerSpellList[i]);
            }
        }

        if (illegalSpellSlots.Count > 0)
        {
            //Replace illegal spells by legal spells that have not been chosen yet
            int currentSpellSlot = 0;
            //Search for first spell that has not already been taken
            for (int i = 0; i < spellregistry.spellList.Count; i++)
            {
                bool isSpellLegal = true;
                //Check against already taken spells
                foreach (A_Spell takenSpell in legalSpells)
                {
                    //if the registry spell has already been taken, flag as illegal an move to next spell in registry
                    if (spellregistry.spellList[i].spellID == takenSpell.spellID)
                    {
                        isSpellLegal = false;
                        break;
                    }
                }

                // Only if the spell is legal, move it into our spellslot
                if (isSpellLegal)
                {
                    //Otherwise, put it into the spellslot
                    mPlayerSpellList[illegalSpellSlots[currentSpellSlot]] = spellregistry.spellList[i];

                    //And move to next illegal spell
                    currentSpellSlot++;
                    if (currentSpellSlot == illegalSpellSlots.Count)
                    {
                        break;
                    }
                }
            }
        }

        if (!spellregistry.ValidateUltimate(mPlayerSpellList[3]))
        {
            mPlayerSpellList[3] = spellregistry.ultimateSpellList[0];
        }

        for (int i = 0; i < selectionHighlights.Length; i++)
        {
            selectionHighlights[i].SetParent(mSpellButtonDictionary[mPlayerSpellList[i]].transform, false);
            selectionHighlights[i].anchoredPosition = Vector2.zero;
        }

    }

    public enum OnSelectDirection
    {
        down,
        up,
        left,
        right
    }
    public void AssignCustomNavigation(Button sourceButton, Button targetButton, OnSelectDirection direction)
    {
        Navigation navigation = sourceButton.navigation;
        navigation.mode = Navigation.Mode.Explicit;
        switch (direction)
        {
            case OnSelectDirection.up:
                navigation.selectOnUp = targetButton;
                break;
            case OnSelectDirection.left:
                navigation.selectOnLeft = targetButton;
                break;
            case OnSelectDirection.right:
                navigation.selectOnRight = targetButton;
                break;
            default:
                navigation.selectOnDown = targetButton;
                break;
        }
        sourceButton.navigation = navigation;
    }

    public void RandomizeSpells()
    {
        mPlayerSpellList = spellregistry.generateRandomSpells();
        ValidateSpellSelection();
    }

    public void OnReadyButtonClicked()
    {
        if(lobbyPlayer.isReady)
        {
            readyButton.SetReadyState(lobbyPlayer.isReady); //We have to do this here as the ready status is set via Command and might cause timing issues if we try to set it after OnUnready
            OnUnready();
        }
        else
        {
            readyButton.SetReadyState(lobbyPlayer.isReady); //We have to do this here as the ready status is set via Command and might cause timing issues if we try to set it after OnReady
            OnReady();
        }
    }

    public void OnReady()
    {
        lobbyPlayer.CmdSetReady(true);
        ValidateSpellSelection();
        //!Save Spells to PlayerPrefs
        PlayerPrefs.SetInt("SpellSlot0", mPlayerSpellList[0].spellID);
        PlayerPrefs.SetInt("SpellSlot1", mPlayerSpellList[1].spellID);
        PlayerPrefs.SetInt("SpellSlot2", mPlayerSpellList[2].spellID);
        PlayerPrefs.SetInt("SpellSlot3", mPlayerSpellList[3].spellID);
        PlayerPrefs.Save();
        mPlayerSpells.CmdUpdateSpells(mPlayerSpellList[0].spellID, mPlayerSpellList[1].spellID, mPlayerSpellList[2].spellID, mPlayerSpellList[3].spellID);
        mPlayerSpells.UpdateSpells(mPlayerSpellList);
        mHUD.GetSpellHUD().UpdateSpellIcons();
    }

    private void OnUnready()
    {
        lobbyPlayer.CmdSetReady(false);
    }

    private void Update()
    {
        if (playerInput.GetButtonDown("SpellSelection1"))
        {
            OnHoverClick(0, false);
        }
        if (playerInput.GetButtonDown("SpellSelection2"))
        {
            OnHoverClick(1, false);
        }
        if (playerInput.GetButtonDown("SpellSelection3"))
        {
            OnHoverClick(2, false);
        }
        if (playerInput.GetButtonDown("Ultimate"))
        {
            OnHoverClick(3, true);
        }
    }

    private void OnHoverClick(int inputIndex, bool needsToBeUltimate)
    {
        var gameObjectHoveredOver = ((Rewired.Integration.UnityUI.RewiredStandaloneInputModule)EventSystem.current.currentInputModule).pointerEventDataOnClick.pointerEnter;
        UISpellButton spellButtonScript = gameObjectHoveredOver.GetComponentInParent<UISpellButton>();
        if (spellButtonScript)
        {
            if (GameManager.instance.localPlayer.playerLobby.isReady)
            {
                unableToAssignPopup.Open();
            }
            else if (spellButtonScript.isUltimate == needsToBeUltimate)
            {
                TradeSpells(spellButtonScript.spell, inputIndex);
            }
        }
    }
}
