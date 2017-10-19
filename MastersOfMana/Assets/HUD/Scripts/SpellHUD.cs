using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellHUD : MonoBehaviour
{
    public List<RectTransform> SpellSlots;

    private Canvas canvas;
    private PlayerScript localPlayer;
    private bool[] OnCooldown = new bool[3];

	// Use this for initialization
	void OnEnable ()
    {
        GameManager.OnGameStarted += Init;
	}

    public void Init()
    {
        //Fill SpellSlots with correct spellIcons
        localPlayer = GameManager.instance.localPlayer;
        SpellSlots[0].GetComponent<Image>().sprite = localPlayer.spellSlot_1.spell.icon;
        SpellSlots[1].GetComponent<Image>().sprite = localPlayer.spellSlot_2.spell.icon;
        SpellSlots[2].GetComponent<Image>().sprite = localPlayer.spellSlot_3.spell.icon;
        canvas = GetComponentInParent<Canvas>();
        canvas.enabled = true;
    }

    public void SetCooldown(int SpellSlotID, PlayerScript.SpellSlot SpellSlot)
    {
        float CooldownPercentage = 0;
        // Calc how much of the cooldown has passed if the Spellcooldown is not 0
        if (SpellSlot.spell.coolDownInSeconds != 0)
        {
            CooldownPercentage = SpellSlot.cooldown / SpellSlot.spell.coolDownInSeconds;
        }


        Image image = SpellSlots[SpellSlotID].GetChild(0).GetComponent<Image>();
        if(image)
        {
            // Did we just finish our Cooldown? --> check if fillAmount will be set to 0 this frame (and wasn't zero before)
            if(image.fillAmount > 0 && CooldownPercentage == 0)
            {
                //SpellSlots[0].GetChild(1).GetComponent<ParticleSystem>().Play(false);// Simulate(100.0f);
            }
            image.fillAmount = CooldownPercentage;
        }
    }

    public void Update()
    {
        if (localPlayer)
        {
            SetCooldown(0, localPlayer.spellSlot_1);
            SetCooldown(1, localPlayer.spellSlot_2);
            SetCooldown(2, localPlayer.spellSlot_3);
        }
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
    }

    private void Reset()
    {
        SpellSlots.Add(transform.GetChild(0).GetComponent<RectTransform>());
        SpellSlots.Add(transform.GetChild(1).GetComponent<RectTransform>());
        SpellSlots.Add(transform.GetChild(2).GetComponent<RectTransform>());
    }
}
