﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpells : NetworkBehaviour {

    //cached instance of the playerscript
    private PlayerScript mPlayer;

    public void Start()
    {
        //cache the playaer for later use
        mPlayer = GetComponent<PlayerScript>();

        //set the currently chosen spell to a default
	    currentSpell = 0;
    }
    /// <summary>
    /// holds references to all the coroutines a spell is running, so they can bes stopped/interrupted w4hen a player is for example hit
    /// and can therefore not continue to cast the spell
    /// </summary>
    private List<Coroutine> mSpellRoutines = new List<Coroutine>();

    public void EnlistSpellRoutine(Coroutine spellRoutine)
    {
        mSpellRoutines.Add(spellRoutine);
    }

    public void FlushSpellroutines()
    {
        for (int i = 0; i < mSpellRoutines.Count; ++i)
        {
            StopCoroutine(mSpellRoutines[i]);
        }
        mSpellRoutines = new List<Coroutine>();
    }

    //spellslots
    public SpellSlot[] spellslot = new SpellSlot[3];

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
        if (currentSpell > 2 || currentSpell < 0)
        {
            currentSpell = 0;
        }
    }

    //choosing a spell
    [Command]
    public void CmdChooseSpellslot(int idx)
    {
        currentSpell = idx;
        mPlayer.RpcSetCastState(CastStateSystem.CastStateID.Normal);
    }


    /// <summary>
    /// Simple Datacontainer (inner class) for a Pair of Spell and cooldown
    /// </summary>
    [System.Serializable]
	public class SpellSlot {
		public A_Spell spell;
		public float cooldown;

        /// <summary>
        /// activates the casting animation, after the spells castduration it activates the 'holding spell' animation
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="castDuration"></param>
        /// <returns></returns>
        private IEnumerator CastRoutine(PlayerScript caster, float castDuration)
        {
            //set caster in 'casting mode'
            caster.RpcSetCastState(CastStateSystem.CastStateID.Resolving);

            yield return new WaitForSeconds(castDuration);
            //resolve the spell
            spell.Resolve(caster);

            //set caster in 'normal mode'
            caster.RpcSetCastState(CastStateSystem.CastStateID.Normal);
        }

        /// <summary>
        /// casts the spell inside the slot and also adjusts the cooldown accordingly
        /// This automatically assumes, that the overlaying PlayerScript's update routine decreases the spellslot's cooldown continuously
        /// This should only be called on the server!!
        /// </summary>
	    public void Cast(PlayerScript caster)
	    {
            if (cooldown <= 0)
            {
                //start the switch to 'holding spell' animation after the castduration
                caster.GetPlayerSpells().EnlistSpellRoutine(caster.StartCoroutine(CastRoutine(caster, spell.castDurationInSeconds)));
            }
        }
	}
}