using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateParalyzed : A_InputState {

    public InputStateParalyzed(PlayerScript player) : base(player) { }

	public override void Move (Vector2 input)
	{
		base.Move (input);

		player.movement.SetFocusActive(player.aim.IsFocused());

		//Vector3 moveForce = World2DToLocal3D(input, player.transform);

		////override moveForce in player script
		//player.movement.SetMoveInput(moveForce);
	}

    public override void UpdateLocal()
    {
		#region Spell Selection
		if (playerInput.GetButtonDown("SpellSelection1")) 
		{
            player.GetPlayerSpells().StopPreview();
			mPreviewActive = false;
        	ChooseSpell(0);
        }
		if (playerInput.GetButtonDown("SpellSelection2")) 
		{
            player.GetPlayerSpells().StopPreview();
			mPreviewActive = false;
			ChooseSpell(1);
        }
		if (playerInput.GetButtonDown("SpellSelection3")) 
		{
            player.GetPlayerSpells().StopPreview();
			mPreviewActive = false;
			ChooseSpell(2);
        }
        #endregion

		if(playerInput.GetButtonDown("ShoulderSwap"))
		{
			player.aim.GetCameraRig().SwapShoulder();
		}

        if ( mPreviewActive &&
            (playerInput.GetButtonUp("QuickCast1") ||
            playerInput.GetButtonUp("QuickCast2") ||
            playerInput.GetButtonUp("QuickCast3") ||
            playerInput.GetButtonUp("CastSpell")))
        {
            player.GetPlayerSpells().StopPreview();
        }

		Aim(playerInput.GetAxis2D("AimHorizontal", "AimVertical"));
    }
}
