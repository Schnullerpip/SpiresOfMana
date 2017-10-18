using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //choosing a spell
    public virtual void ChooseSpell_1() 
	{  
        player.CmdChooseSpellslot_1();
	}

    public virtual void ChooseSpell_2()
    {
        player.CmdChooseSpellslot_2();
    }

    public virtual void ChooseSpell_3()
    {
        player.CmdChooseSpellslot_3();
    }

    //casting the chosen spell
    public virtual void CastSpell()
    {
        player.castStateSystem.current.CastCmdSpell();
    }

    //resolving the chosen spell
    public virtual void ResolveSpell() 
	{  
        player.castStateSystem.current.ResolveCmdSpell();
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
