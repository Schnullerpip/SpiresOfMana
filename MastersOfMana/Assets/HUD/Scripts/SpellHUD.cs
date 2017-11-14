using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellHUD : MonoBehaviour
{
    public List<RectTransform> spellSlots;

    private Canvas canvas;
    private PlayerScript localPlayer;

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
        GameManager.OnGameStarted += Init;
    }

    public void Init()
    {
        localPlayer = GameManager.instance.localPlayer;

        //Prefetch all the needed children
        spellHighlights.Add(spellSlots[0].GetChild(0).GetComponentInParent<Image>());
        spellHighlights.Add(spellSlots[1].GetChild(0).GetComponentInParent<Image>());
        spellHighlights.Add(spellSlots[2].GetChild(0).GetComponentInParent<Image>());
        spellHighlights.Add(spellSlots[3].GetChild(0).GetComponentInParent<Image>());

        spellIcons.Add(spellSlots[0].GetChild(1).GetComponentInParent<Image>());
        spellIcons.Add(spellSlots[1].GetChild(1).GetComponentInParent<Image>());
        spellIcons.Add(spellSlots[2].GetChild(1).GetComponentInParent<Image>());
        spellIcons.Add(spellSlots[3].GetChild(1).GetComponentInParent<Image>());

        spellCooldowns.Add(spellSlots[0].GetChild(2).GetComponentInParent<Image>());
        spellCooldowns.Add(spellSlots[1].GetChild(2).GetComponentInParent<Image>());
        spellCooldowns.Add(spellSlots[2].GetChild(2).GetComponentInParent<Image>());
        spellCooldowns.Add(spellSlots[3].GetChild(2).GetComponentInParent<Image>());

        //Fill SpellSlots with correct spellIcons
        spellIcons[0].sprite = localPlayer.GetPlayerSpells().spellslot[0].spell.icon;
        spellIcons[1].sprite = localPlayer.GetPlayerSpells().spellslot[1].spell.icon;
        spellIcons[2].sprite = localPlayer.GetPlayerSpells().spellslot[2].spell.icon;
        spellIcons[3].sprite = localPlayer.GetPlayerSpells().spellslot[3].spell.icon;

        //set selected spell
        displayedCurrentSpell = localPlayer.GetPlayerSpells().GetCurrentspellslotID();
        spellHighlights[displayedCurrentSpell].GetComponent<Image>().enabled = true;
        canvas = GetComponentInParent<Canvas>();
        canvas.enabled = true;
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

    public void Update()
    {
        if (localPlayer)
        {
            SetCooldown(0, localPlayer.GetPlayerSpells().spellslot[0]);
            SetCooldown(1, localPlayer.GetPlayerSpells().spellslot[1]);
            SetCooldown(2, localPlayer.GetPlayerSpells().spellslot[2]);
            SetCurrentSpell(localPlayer.GetPlayerSpells().GetCurrentspellslotID());
        }
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
    }

    private void Reset()
    {
        spellSlots.Add(transform.GetChild(0).GetComponent<RectTransform>());
        spellSlots.Add(transform.GetChild(1).GetComponent<RectTransform>());
        spellSlots.Add(transform.GetChild(2).GetComponent<RectTransform>());
    }
}
