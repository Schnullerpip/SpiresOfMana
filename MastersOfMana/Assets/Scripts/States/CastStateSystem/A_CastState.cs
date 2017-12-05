using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_CastState : A_State{
    public A_CastState(PlayerScript player) : base(player)
    {
        castDurationCount = 0;
    }

    /// <summary>
    /// will store the time in milliseconds, that a character is casting already, to validate that a spell can resolve now
    /// </summary>
    protected float castDurationCount;

    public float GetCastDurationCount()
    {
        return castDurationCount;
    }

    public void ResetCastDurationCount()
    {
        castDurationCount = 0;
    }

    /*behaviour distinction
     * those are keptempty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */

    /// <summary>
    /// should incerase the castDurationCount by the deltatime
    /// </summary>
    public virtual void IncrementCastDuration() { }

    /// <summary>
    /// the method that handles reducing the spells' cooldowns so they can be cast again
    /// </summary>
    public virtual void ReduceCooldowns() {}

    /// <summary>
    /// will invoke the spell that is maped at spellslot 1
    /// </summary>
    public virtual void CastCmdSpell()
    {
        var camera = player.aim.GetCameraRig().GetCamera();

		player.CmdResolveSpell(
			camera.transform.position, 
			camera.transform.forward,
			player.aim.currentLookRotation
		);
    }
}
