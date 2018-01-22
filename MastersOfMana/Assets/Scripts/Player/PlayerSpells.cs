using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpells : NetworkBehaviour {

    //cached instance of the playerscript
    private PlayerScript mPlayer;
    public SpellRegistry spellregistry;

    [Header("SFX")]
	public AudioSource audioSource;

	public AudioClip castFailedSFX;
	public AudioClip cooldownDoneSFX;

	[Header("Energy")]

    [SyncVar(hook = "UltimateEnergyHook")]
    public float ultimateEnergy = 0;
    public float ultimateEnergyThreshold = 30;

    public delegate void UltiChange(float newValue);
    public UltiChange onUltiChange;

    private float mClampedEnergy;

    void UltimateEnergyHook(float newVal)
    {
        newVal = Mathf.Clamp(newVal, 0, ultimateEnergyThreshold);

        //only trigger if the value actually changed within the threshold
        if(!Mathf.Approximately(newVal, mClampedEnergy))
        {
            if(onUltiChange != null)
            {
                onUltiChange(newVal);
            }  
        }

        ultimateEnergy = newVal;
        mClampedEnergy = newVal;		
    }

    public void Start()
    {
        //cache the playaer for later use
        mPlayer = GetComponent<PlayerScript>();
		spellslot[0].spell = spellregistry.GetSpellByID(PlayerPrefs.GetInt("SpellSlot0", 0));
		spellslot[1].spell = spellregistry.GetSpellByID(PlayerPrefs.GetInt("SpellSlot1", 1));
		spellslot[2].spell = spellregistry.GetSpellByID(PlayerPrefs.GetInt("SpellSlot2", 2));
		spellslot[3].spell = spellregistry.GetSpellByID(PlayerPrefs.GetInt("SpellSlot3", 101));
        //set the currently chosen spell to a default
	    currentSpell = 0;
    }

	public void OnEnable()
	{
		GameManager.OnRoundStarted += RoundStarted;
        GameManager.OnRoundEnded += RoundEnded;
    }

	public void OnDisable()
	{
		GameManager.OnRoundStarted -= RoundStarted;
        GameManager.OnRoundEnded += RoundEnded;
    }

    public void RoundEnded()
    {
        foreach(SpellSlot slot in spellslot)
        {
            slot.cooldown = 0;
        }
		ultimateEnergy = 0;
    }

    void RoundStarted()
	{
		ultimateEnergy = 0;
	}

    //spellslots
    public SpellSlot[] spellslot = new SpellSlot[4];

    //references the currently chosen spell, among the three available spellslots
    [SyncVar]
    public int currentSpell;

    public SpellSlot GetCurrentspell()
    {
        return spellslot[currentSpell];
    }

    public int GetCurrentspellslotID()
    {
        return currentSpell;
    }

    public void SetCurrentSpellslotID(int idx)
    {
        currentSpell = idx;
        if (currentSpell > 3 || currentSpell < 0)
        {
            currentSpell = 0;
        }
    }

	public void PreviewCurrentSpell()
	{
		A_SpellBehaviour spell = GetCurrentspell().spell.SpellBehaviours[0];

		if(spell)
		{
			spell.Preview(mPlayer);
		}
	}

	public void StopPreview()
	{
		A_SpellBehaviour spell = GetCurrentspell().spell.SpellBehaviours[0];

		if(spell)
		{
			spell.StopPreview(mPlayer);
		}
	}

    //choosing a spell
    [Command]
    public void CmdChooseSpellslot(int idx)
    {
        currentSpell = idx;
        mPlayer.RpcSetCastState(CastStateSystem.CastStateID.Normal);
    }

	public void PlayFailSFX()
	{
		audioSource.pitch = 1;
		audioSource.PlayOneShot(castFailedSFX);
	}

	public void PlayCooldownDoneSFX()
	{
		audioSource.pitch = 1;
		audioSource.PlayOneShot(cooldownDoneSFX);
	}

    /// <summary>
    /// This method actually updates the spells
    /// </summary>
    /// <param name="spell1"></param>
    /// <param name="spell2"></param>
    /// <param name="spell3"></param>
    [Command]
    public void CmdUpdateSpells(int spell1, int spell2, int spell3, int spell4)
    {
        //Prototype.NetworkLobby.LobbyManager NetworkManager = Prototype.NetworkLobby.LobbyManager.s_Singleton;
        //if (NetworkManager)
        //{
            if (spellregistry)
            {
                spellslot[0].spell = spellregistry.GetSpellByID(spell1);
                spellslot[1].spell = spellregistry.GetSpellByID(spell2);
                spellslot[2].spell = spellregistry.GetSpellByID(spell3);
                spellslot[3].spell = spellregistry.GetSpellByID(spell4);
            }
        //}
    }

    public void UpdateSpells(List<A_Spell> spells)
    {
        for(int i = 0; i < spells.Count; i++)
        {
            spellslot[i].spell = spells[i];
        }
    }

    ///// <summary>
    ///// Update spells on client side
    ///// </summary>
    ///// <param name="spell1"></param>
    ///// <param name="spell2"></param>
    ///// <param name="spell3"></param>
    //[ClientRpc]
    //public void RpcUpdateSpells(int spell1, int spell2, int spell3, int spell4)
    //{
    //    UpdateSpells(spell1, spell2, spell3, spell4);
    //}

    /// <summary>
    /// Simple Datacontainer (inner class) for a Pair of Spell and cooldown
    /// </summary>
    [System.Serializable]
	public class SpellSlot {
		public A_Spell spell;
		public float cooldown;
        public bool isUltimateSpellSlot = false;

		/// <summary>
		/// Reduces the cooldown by delta. Returns true when the cooldown reached 0 in this iteration (Fires only once)
		/// </summary>
		/// <returns><c>true</c>, if cooldown has reached 0 in this iteration, <c>false</c> otherwise.</returns>
		/// <param name="delta">Delta.</param>
		public bool ReduceCooldown(float delta)
		{
			if(cooldown > 0)
			{
				if((cooldown -= delta) <= 0)
				{
					cooldown = 0;
					return true;
				}
			}
			return false;
		}

        public void ResetCooldown()
        {
            cooldown = spell.coolDownInSeconds;
        }

        /// <summary>
        /// casts the spell inside the slot and also adjusts the cooldown accordingly
        /// This automatically assumes, that the overlaying PlayerScript's update routine decreases the spellslot's cooldown continuously
        /// This should only be called on the server!!
        /// </summary>
	    public bool Cast(PlayerScript caster)
	    {
            if (cooldown <= 0)
            {
                if(isUltimateSpellSlot)
                {
                    //Only one ultimate can be active at the same time
                    if(GameManager.instance.isUltimateActive)
                    {
                        return false;
                    }
                    caster.GetPlayerSpells().ultimateEnergy = 0;
                }
                //set caster in 'casting mode'
                //caster.RpcSetCastState(CastStateSystem.CastStateID.Resolving);
                caster.RpcCastAndAnimate(spell.castAnimationID);
                //resolve the spell
                spell.Resolve(caster);
                //set caster in 'normal mode'
                caster.RpcSetCastState(CastStateSystem.CastStateID.Normal);

				return true;
            }
			return false;
        }
	}
}