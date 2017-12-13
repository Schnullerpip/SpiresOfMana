using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// abstract State, that defines default behaviour for substates and provides the base constructor
/// </summary>
public abstract class A_InputState : A_State{

	protected Rewired.Player playerInput;

    public A_InputState(PlayerScript player) : base(player) 
	{ 
		playerInput = player.GetRewired();
	}

    /*behaviour distinction
     * those are kept empty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */

	protected bool mPreviewActive;
	protected bool mFocus;

    public override void UpdateLocal()
    {
		#region Spell Selection
		if (playerInput.GetButtonDown("SpellSelection1")) 
		{
			SetPreview(false);
        	ChooseSpell(0);
        }
		if (playerInput.GetButtonDown("SpellSelection2")) 
		{
			SetPreview(false);
			ChooseSpell(1);
        }
		if (playerInput.GetButtonDown("SpellSelection3")) 
		{
			SetPreview(false);
			ChooseSpell(2);
        }
        #endregion

		#region Quickcast
		if(playerInput.GetButtonDown("QuickCast1"))
		{
			TogglePreview();
			ChooseSpell(0);
		}
		else if(playerInput.GetButtonUp("QuickCast1") && mPreviewActive)
		{
			CastSpell();
		}

		if(playerInput.GetButtonDown("QuickCast2"))
		{
			TogglePreview();
			ChooseSpell(1);
		}
		else if(playerInput.GetButtonUp("QuickCast2") && mPreviewActive)
		{
			CastSpell();
		}

		if(playerInput.GetButtonDown("QuickCast3"))
		{
			TogglePreview();
			ChooseSpell(2);
		}
		else if(playerInput.GetButtonUp("QuickCast3") && mPreviewActive)
		{
			CastSpell();
		}

		if (player.GetRewired().GetButtonDown("Ultimate"))
		{
			if(player.GetPlayerSpells().ultimateEnergy >= player.GetPlayerSpells().ultimateEnergyThreshold)
			{
				TogglePreview();
				ChooseSpell(3);
			}
		}
		else if(playerInput.GetButtonUp("Ultimate") && mPreviewActive)
		{
			CastSpell();
		}
			
		#endregion

		if(playerInput.GetButtonDown("CastSpell"))
		{
			SetPreview(true);
		}
		if(mPreviewActive && playerInput.GetButtonUp("CastSpell"))
		{
			CastSpell();
		}

		if(mPreviewActive)
		{
			player.GetPlayerSpells().PreviewCurrentSpell();
		}

		//store the input values
		Vector2 movementInput = playerInput.GetAxis2D("MoveHorizontal", "MoveVertical");
		Move(movementInput);


		if(playerInput.GetButtonDown("ShoulderSwap"))
		{
			player.aim.GetCameraRig().SwapShoulder();
		}

		//propergate various inputs to the statesystems
		if(playerInput.GetButtonDown("Jump"))
		{
			player.movement.Jump();
		}
			
		if(playerInput.GetButtonDown("Focus"))
		{
			mFocus = !mFocus;
			if(mFocus)
			{
				player.aim.StartFocus(player);
			}
			else
			{
				player.aim.StopFocus();
			}
		}
		else if(playerInput.GetButtonShortPressUp("Focus"))
		{
			mFocus = false;
			player.aim.StopFocus();
		}

		//store the aim input, either mouse or right analog stick
		Vector2 aimInput = playerInput.GetAxis2D("AimHorizontal", "AimVertical");
//		aimInput = Vector3.ClampMagnitude(aimInput,1); //TODO: delete maybe?

		Aim(aimInput);

    }

	void TogglePreview()
	{
		mPreviewActive = !mPreviewActive;
		player.GetPlayerAnimation().HoldingSpell(mPreviewActive);
	}

	void SetPreview(bool value)
	{
		mPreviewActive = value;
		player.GetPlayerAnimation().HoldingSpell(mPreviewActive);
	}

	public virtual void Move(Vector2 input) { }

	public virtual void Aim(Vector2 aimInput) 
	{
//		player.aim.ValidateFocusTargetView();

		if(player.aim.HasFocusTarget())
		{
			//TODO: get actual rewired controller type
//			player.aim.RefineAim(aimInput, Rewired.ControllerType.Mouse);
			player.aim.LookAtFocusTarget();
		}
		else
		{
//			player.aim.ResetRefinement();
			player.aim.Aim(aimInput);
		}
	}

    public virtual void ChooseSpell(int idx)
    {
		PlayerSpells playerSpells = player.GetPlayerSpells();
		playerSpells.StopPreview();
        playerSpells.SetCurrentSpellslotID(idx);
        playerSpells.CmdChooseSpellslot(idx);
    }

    //casting the chosen spell
    public virtual void CastSpell()
    {
		mPreviewActive = false;
		player.GetPlayerSpells().StopPreview();
        player.castStateSystem.current.CastCmdSpell();
		if(player.GetPlayerSpells().GetCurrentspell().cooldown > 0)
		{
			player.GetPlayerAnimation().HoldingSpell(false);
			player.GetPlayerSpells().PlayFailSFX();
		}
    }

	protected Vector3 World2DToLocal3D (Vector2 world2D, Transform transform)
	{
		//from global to local space
		return transform.TransformDirection (world2D.x, 0, world2D.y);
	}
}
