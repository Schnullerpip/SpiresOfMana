using System.Collections;
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

    public override void UpdateLocal()
    {
        //poll the input coming from rewired

        //poll wheather a spell was chosen
        if (player.GetRewired().GetButtonDown("ChooseSpell1")) {
           ChooseSpell(0);
        }
		if (player.GetRewired().GetButtonDown("ChooseSpell2")) {
            ChooseSpell(1);
        }
		if (player.GetRewired().GetButtonDown("ChooseSpell3")) {
            ChooseSpell(2);
        }




		//store the input values
		Vector2 movementInput = player.GetRewired().GetAxis2D("MoveHorizontal", "MoveVertical");
		movementInput = Vector3.ClampMagnitude(movementInput,1);

		if(player.GetRewired().GetButtonDown("ShoulderSwap"))
		{
			player.aim.cameraRig.SwapShoulder();
		}

		//propergate various inputs to the statesystems
		#region Input
		if(player.GetRewired().GetButtonDown("Jump"))
		{
			Jump();
		}


		Move(movementInput);

		//TODO: define mouse & keyboard / controller schemes, "CastSpell" not final axis name
		if(player.GetRewired().GetButtonDown("CastSpell"))
		{
			CastSpell();
		}

		Move(movementInput);

		if(player.GetRewired().GetButtonDown("Focus"))
		{
			StartFocus();
		}

		if(player.GetRewired().GetButtonUp("Focus"))
		{
			StopFocus();
		}

		//store the aim input, either mouse or right analog stick
		Vector2 aimInput = player.GetRewired().GetAxis2D("AimHorizontal", "AimVertical");
//		aimInput = Vector3.ClampMagnitude(aimInput,1); //TODO: delete maybe?

		Aim(aimInput);

		#endregion
    }

	public virtual void Move(Vector2 input) { }

	public virtual void Aim(Vector2 aimInput) 
	{
		player.aim.ValidateFocusTargetView();

		if(player.aim.HasFocusTarget())
		{
			//TODO: get actual rewired controller type
			player.aim.RefineAim(aimInput, Rewired.ControllerType.Mouse);
			player.aim.RotateTowardsFocusTarget();
		}
		else
		{
			player.aim.ResetRefinement();
			player.aim.Aim(aimInput);
		}
	}

    public virtual void Jump() 
	{
		player.movement.Jump();
	}

    public virtual void ChooseSpell(int idx)
    {
        player.GetPlayerSpells().SetCurrentSpellslotID(idx);
        player.GetPlayerSpells().CmdChooseSpellslot(idx);
    }

    //casting the chosen spell
    public virtual void CastSpell()
    {
        player.castStateSystem.current.CastCmdSpell();
    }

	public virtual void StartFocus()
	{
		player.aim.StartFocus();
	}

	public virtual void StopFocus()
	{
		player.aim.StopFocus();
	}
}
