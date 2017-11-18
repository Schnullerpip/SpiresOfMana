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
	protected bool mSprint;

    public override void UpdateLocal()
    {
		#region Spell Selection
		if (playerInput.GetButtonDown("SpellSelection1")) 
		{
			mPreviewActive = false;
        	ChooseSpell(0);
        }
		if (playerInput.GetButtonDown("SpellSelection2")) 
		{
			mPreviewActive = false;
			ChooseSpell(1);
        }
		if (playerInput.GetButtonDown("SpellSelection3")) 
		{
			mPreviewActive = false;
			ChooseSpell(2);
        }
        #endregion

		#region Quickcast
		if(playerInput.GetButtonDown("QuickCast1"))
		{
			mPreviewActive = !mPreviewActive;
			ChooseSpell(0);
		}
		else if(playerInput.GetButtonUp("QuickCast1") && mPreviewActive)
		{
			mPreviewActive = false;
			CastSpell();
		}

		if(playerInput.GetButtonDown("QuickCast2"))
		{
			ChooseSpell(1);
			mPreviewActive = !mPreviewActive;
		}
		else if(playerInput.GetButtonUp("QuickCast2") && mPreviewActive)
		{
			mPreviewActive = false;
			CastSpell();
		}

		if(playerInput.GetButtonDown("QuickCast3"))
		{
			ChooseSpell(2);
			mPreviewActive = !mPreviewActive;
		}
		else if(playerInput.GetButtonUp("QuickCast3") && mPreviewActive)
		{
			mPreviewActive = false;
			CastSpell();
		}

		if (player.GetRewired().GetButtonDown("Ultimate"))
		{
			if(player.GetPlayerSpells().ultimateEnergy >= player.GetPlayerSpells().ultimateEnergyThreshold)
			{
				ChooseSpell(3);
				mPreviewActive = !mPreviewActive;
			}
		}
		else if(playerInput.GetButtonUp("Ultimate") && mPreviewActive)
		{
			mPreviewActive = false;
			CastSpell();
		}

		#endregion

		if(playerInput.GetButtonDown("CastSpell"))
		{
			mPreviewActive = true;
		}
		if(Input.GetKeyDown(KeyCode.P) || (playerInput.GetButtonUp("CastSpell") && mPreviewActive))
		{
			mPreviewActive = false;
			CastSpell();
		}

		if(mPreviewActive)
		{
			player.GetPlayerSpells().PreviewCurrentSpell();
		}
		else
		{
			player.GetPlayerSpells().StopPreview();
		}

		if(playerInput.GetButtonDown("Sprint"))
		{
			mSprint = !mSprint;
			if(mSprint)
			{
				Debug.Log("Start Sprint");
			}
			else
			{
				Debug.Log("Stop Sprint");
			}
		}
		else if(playerInput.GetButtonShortPressUp("Sprint"))
		{
			mSprint = false;
			Debug.Log("Stop Sprint");
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
			Jump();
		}
			
		if(playerInput.GetButtonDown("Focus"))
		{
			mFocus = !mFocus;
			if(mFocus)
			{
				StartFocus();
			}
			else
			{
				StopFocus();
			}
		}
		else if(playerInput.GetButtonShortPressUp("Focus"))
		{
			mFocus = false;
			StopFocus();
		}

		//store the aim input, either mouse or right analog stick
		Vector2 aimInput = playerInput.GetAxis2D("AimHorizontal", "AimVertical");
//		aimInput = Vector3.ClampMagnitude(aimInput,1); //TODO: delete maybe?

		Aim(aimInput);

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
		player.aim.StartFocus(player);
	}

	public virtual void StopFocus()
	{
		player.aim.StopFocus();
	}

	protected Vector3 World2DToLocal3D (Vector2 world2D, Transform transform)
	{
		//from global to local space
		return transform.TransformDirection (world2D.x, 0, world2D.y);
	}
}
