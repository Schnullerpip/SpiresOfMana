﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// abstract State, that defines default behaviour for substates and provides the base constructor
/// </summary>
public abstract class A_InputState : A_State{

    public A_InputState(PlayerScript player) : base(player) { }

    /*behaviour distinction
     * those are kept empty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */
	public virtual void Move(Vector2 input) { }

	public virtual void Aim(Vector2 aimInput) 
	{
		player.ValidateFocusTargetView();

		if(player.HasFocusTarget())
		{
			player.RefineAim(aimInput);
			player.RotateTowardsFocusTarget();
		}
		else
		{
			player.ResetRefinement();
			player.Aim(aimInput);
		}
	}

    public virtual void Jump() 
	{
		player.Jump();
	}

    public virtual void ChooseSpell(int idx)
    {
        player.SetCurrentSpellslotID(idx);
        player.CmdChooseSpellslot(idx);
    }

    //casting the chosen spell
    public virtual void CastSpell()
    {
        player.castStateSystem.current.CastCmdSpell();
    }

	public virtual void StartFocus()
	{
		player.StartFocus();
	}

	public virtual void StopFocus()
	{
		player.StopFocus();
	}
}
