using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellHUD : MonoBehaviour
{
    public List<SpellSlotHUD> spellSlots;
    public Image ultimateEnergyProgess;
    public Color ultimateEnergyDefaultColor;
    public Color ultimateEnergyFullColor;

    private PlayerSpells localPlayerSpells;

    private List<Image> spellIcons = new List<Image>();
    private List<Image> spellHighlights = new List<Image>();
    private List<Image> spellCooldowns = new List<Image>();
    public Image targetCooldown;

    public delegate void CooldownFinished();
    public event CooldownFinished OnCooldownFinished;

    private int displayedCurrentSpell;

    // Use this for initialization
    void OnEnable ()
    {
        //GameManager.OnGameStarted += Init;
        GameManager.OnLocalPlayerDead += LocalPlayerDead;
        Init();
    }

    public void Init()
    {
        localPlayerSpells = GameManager.instance.localPlayer.GetPlayerSpells();

        foreach(SpellSlotHUD spellslot in spellSlots)
        {
            spellHighlights.Add(spellslot.highlight);
            spellIcons.Add(spellslot.icon);
            spellCooldowns.Add(spellslot.cooldownImage);
            spellIcons[spellIcons.Count-1].sprite = localPlayerSpells.spellslot[spellIcons.Count-1].spell.icon;
        }

        //set selected spell
        displayedCurrentSpell = localPlayerSpells.GetCurrentspellslotID();
        spellHighlights[displayedCurrentSpell].GetComponent<Image>().enabled = true;
    }

    public void UpdateSpellIcons()
    {
        for(int i = 0; i < spellSlots.Count; i++)
        {
            spellIcons[i].sprite = localPlayerSpells.spellslot[i].spell.icon;
        }
    }

    public void SetCooldown(int SpellSlotID, PlayerSpells.SpellSlot SpellSlot)
    {
        float CooldownPercentage = 0;
        // Calc how much of the cooldown has passed if the Spellcooldown is not 0
        if (SpellSlot.spell.coolDownInSeconds != 0)
        {
            CooldownPercentage = SpellSlot.cooldown / SpellSlot.spell.coolDownInSeconds;
        }

        // Did we just finish our Cooldown? --> check if fillAmount will be set to 0 this frame (and wasn't zero before)
        if(spellCooldowns[SpellSlotID].fillAmount > 0 && CooldownPercentage == 0)
        {
            if(OnCooldownFinished != null)
            {
                OnCooldownFinished();
            }
            //SpellSlots[0].GetChild(1).GetComponent<ParticleSystem>().Play(false);// Simulate(100.0f);
        }
        spellCooldowns[SpellSlotID].fillAmount = CooldownPercentage;
        if(SpellSlotID == displayedCurrentSpell)
        {
            targetCooldown.fillAmount = 1-CooldownPercentage;
        }
    }

    public void SetCurrentSpell(int currentSpell)
    {
        if(currentSpell == displayedCurrentSpell)
        {
            return;
        }
        //disable old highlight
        spellHighlights[displayedCurrentSpell].GetComponent<Image>().enabled = false;

        //enable new highlight
        displayedCurrentSpell = currentSpell;
        spellHighlights[displayedCurrentSpell].GetComponent<Image>().enabled = true;
    }

    public void UpdateSlider(float ultimateEnergyPoints, float maximumUltimateEnergyPoints)
    {
        if(ultimateEnergyPoints < maximumUltimateEnergyPoints)
        {
            ultimateEnergyProgess.color = ultimateEnergyDefaultColor;
            ultimateEnergyProgess.fillAmount = ultimateEnergyPoints / maximumUltimateEnergyPoints;
        }
        else
        {
            ultimateEnergyProgess.fillAmount = 1;
            ultimateEnergyProgess.color = ultimateEnergyFullColor;
        }
    }

    public void Update()
    {
        if (localPlayerSpells)
        {
            SetCooldown(0, localPlayerSpells.spellslot[0]);
            SetCooldown(1, localPlayerSpells.spellslot[1]);
            SetCooldown(2, localPlayerSpells.spellslot[2]);
            SetCurrentSpell(localPlayerSpells.GetCurrentspellslotID());
            UpdateSlider(localPlayerSpells.ultimateEnergy, localPlayerSpells.ultimateEnergyThreshold);
        }
    }

    public void OnSpellButtonClicked(int spellButton)
    {
        localPlayerSpells.SetCurrentSpellslotID(spellButton);
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
        GameManager.OnLocalPlayerDead -= LocalPlayerDead;
    }

    private void LocalPlayerDead()
    {
        foreach (SpellSlotHUD spellslot in spellSlots)
        {
            spellslot.setActive(false);
        }
    }
}
