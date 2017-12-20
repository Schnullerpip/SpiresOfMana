using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellSelectionPanel : MonoBehaviour {

    public SpellRegistry spellregistry;
    public RectTransform spellList;
    public Button spellButtonPrefab;
    private PlayerSpells mPlayerSpells;
    private List<A_Spell> mPlayerSpellList = new List<A_Spell>();
    private HUD mHUD;
    public bool showUltimates = false;
    private bool mShowUltimates = false;
    private bool mUltimatesShown = false;
    private List<Button> spellButtons = new List<Button>();

    private void OnEnable()
    {
        Init();
        spellButtons[0].OnSelect(null);
    }

    public void Init()
    {
        mPlayerSpells = GameManager.instance.localPlayer.GetPlayerSpells();
        mPlayerSpellList = GetPlayerSpells();
        mHUD = GetComponentInParent<HUD>();
        fillList();
        // This will make sure the first spell is highlighted when opening
        GetComponentInParent<MultipleMenuInput>().firstSelected = spellButtons[0].gameObject;
    }

    private void fillList()
    {
        clearList();
        List<A_Spell> spells = spellregistry.spellList;
        if (mShowUltimates)
        {
            spells = spellregistry.ultimateSpellList;
        }
        for (int i = 0; i < spells.Count; i++)
        {
            A_Spell spell = spells[i];
            Button spellButton = GameObject.Instantiate(spellButtonPrefab);
            Image image = spellButton.transform.GetChild(0).GetComponent<Image>();
            if (image)
            {
                image.sprite = spell.icon;
            }
            spellButton.transform.SetParent(spellList);
            spellButton.transform.localScale = new Vector3(1, 1, 1);
            spellButton.onClick.AddListener(delegate { OnClickSpellButton(spell); });
            spellButtons.Add(spellButton);
        }
    }

    private void clearList()
    {
        foreach (Button button in spellButtons)
        {
            Destroy(button.gameObject);
        }
        spellButtons.Clear();
    }

    private List<A_Spell> GetPlayerSpells()
    {
        List<A_Spell> playerSpells = new List<A_Spell>();
        foreach(PlayerSpells.SpellSlot slot in mPlayerSpells.spellslot)
        {
            playerSpells.Add(slot.spell);
        }
        return playerSpells;
    }

    private void SetPlayerSpells(List<A_Spell> spells)
    {
        for(int i = 0; i < spells.Count; i++)
        {
            mPlayerSpells.spellslot[i].spell = spells[i];
        }
    }

    private void OnClickSpellButton(A_Spell spell)
    {
        tradeSpells(spell);
        //Cancel();
    }

    public void tradeSpells(A_Spell spell)
    {
        for (int i = 0; i < mPlayerSpellList.Count-1; i++)
        {
            //Check if the chosen spell is already used
            if (mPlayerSpellList[i] == spell && i != mPlayerSpells.currentSpell)
            {
                //If the spell is already used, just swap the spell to change with the old position of the new spell
                mPlayerSpellList[i] = mPlayerSpellList[mPlayerSpells.currentSpell];
                break;
            }
        }
        mPlayerSpellList[mPlayerSpells.currentSpell] = spell;
        SetPlayerSpells(mPlayerSpellList);
        ValidateSpellSelection();
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

        SetPlayerSpells(mPlayerSpellList);
        mHUD.GetSpellHUD().UpdateSpellIcons();
    }

    public void Update()
    {
        mShowUltimates = mPlayerSpells.currentSpell == 3;
        if((mShowUltimates && !mUltimatesShown) || (!mShowUltimates && mUltimatesShown))
        {
            mUltimatesShown = mShowUltimates;
            fillList();
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
        SetPlayerSpells(mPlayerSpellList);
        mHUD.GetSpellHUD().UpdateSpellIcons();
    }

    public void OnReady()
    {
        ValidateSpellSelection();
        //Save Spells to PlayerPrefs
        PlayerPrefs.SetInt("SpellSlot0", mPlayerSpellList[0].spellID);
        PlayerPrefs.SetInt("SpellSlot1", mPlayerSpellList[1].spellID);
        PlayerPrefs.SetInt("SpellSlot2", mPlayerSpellList[2].spellID);
        PlayerPrefs.SetInt("SpellSlot3", mPlayerSpellList[3].spellID);
        PlayerPrefs.Save();
        mPlayerSpells.CmdUpdateSpells(mPlayerSpellList[0].spellID, mPlayerSpellList[1].spellID, mPlayerSpellList[2].spellID, mPlayerSpellList[3].spellID);
    }

    public void OnDisable()
    {
        clearList();
    }
}
