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

    public delegate void CooldownFinished();
    public event CooldownFinished OnCooldownFinished;

    public InputImage[] inputImages;

    private int displayedCurrentSpell;

    private static readonly System.Guid guid_PS4 = new System.Guid("cd9718bf-a87a-44bc-8716-60a0def28a9f");
    private static readonly System.Guid guid_PS3 = new System.Guid("71dfe6c8-9e81-428f-a58e-c7e664b7fbed");
    //private static readonly System.Guid guid_XB360 = new System.Guid("d74a350e-fe8b-4e9e-bbcd-efff16d34115");
    //private static readonly System.Guid guid_XB1 = new System.Guid("19002688-7406-4f4a-8340-8d25335406c8");

    //private Rewired.Player mRewired;

    private void Awake()
    {
        //mRewired = Rewired.ReInput.players.GetPlayer(0);
        Rewired.ActiveControllerChangedDelegate onControllerChangedDelegate = OnControllerChanged;
        Rewired.ReInput.controllers.AddLastActiveControllerChangedDelegate(onControllerChangedDelegate);
    }


    void OnControllerChanged(Rewired.Controller controller)
    {
        if (controller.type == Rewired.ControllerType.Mouse || controller.type == Rewired.ControllerType.Keyboard)
        {
            foreach (var img in inputImages)
            {
                img.image.sprite = img.mouseAndKeyboardSprite;
            }
        }
        else if (controller.type == Rewired.ControllerType.Joystick)
        {
            System.Guid guid = Rewired.ReInput.controllers.GetJoystick(controller.id).hardwareTypeGuid;
            if(guid == guid_PS3 || guid == guid_PS4)
            {
                foreach (var img in inputImages)
                {
                    img.image.sprite = img.psControllerSprite;
                }
            }
            else
            {
                foreach (var img in inputImages)
                {
                    img.image.sprite = img.genericControllerSprite;
                }           
            }
        }
    }

    // Use this for initialization
    void OnEnable ()
    {
        GameManager.OnLocalPlayerDead += LocalPlayerDead;
    }

    public void Start()
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
        if (SpellSlot.spell.coolDownInSeconds > float.Epsilon)
        {
            CooldownPercentage = SpellSlot.cooldown / SpellSlot.spell.coolDownInSeconds;
        }

        // Did we just finish our Cooldown? --> check if fillAmount will be set to 0 this frame (and wasn't zero before)
        if(spellCooldowns[SpellSlotID].fillAmount > 0 && CooldownPercentage <= float.Epsilon)
        {
            if(OnCooldownFinished != null)
            {
                OnCooldownFinished();
            }
        }
        spellCooldowns[SpellSlotID].fillAmount = CooldownPercentage;
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

    private float mLastUlitEnergy = 0.0f;
    public UniformScaler ultiScaler;

    public void UpdateSlider(float ultimateEnergyPoints, float maximumUltimateEnergyPoints)
    {
        if(ultimateEnergyPoints > mLastUlitEnergy)
        {
            ultiScaler.PlayOnce();
        }
		mLastUlitEnergy = ultimateEnergyPoints;

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
        GameManager.OnLocalPlayerDead -= LocalPlayerDead;
    }

    private void LocalPlayerDead()
    {
        gameObject.SetActive(false);
    }

    [System.Serializable]
    public struct InputImage
    {
        public Image image;
        public Sprite mouseAndKeyboardSprite;
        public Sprite genericControllerSprite;
        public Sprite psControllerSprite;
    }
}
