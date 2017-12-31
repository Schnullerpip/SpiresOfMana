using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateNormal : A_InputState
{
    public InputStateNormal(PlayerScript player) : base(player) { }

    public override void UpdateLocal()
    {

        base.UpdateLocal();

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
    public override void Move (Vector2 input)
	{
		base.Move (input);
		player.movement.SetFocusActive(player.aim.IsFocused());

		Vector3 moveForce = World2DToLocal3D(input, player.transform);

		//override moveForce in player script
		player.movement.SetMoveInput(moveForce);
	}
}