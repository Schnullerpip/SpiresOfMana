using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateNoMovement : A_InputState {

    public InputStateNoMovement(PlayerScript player) : base(player) { }


    public override void UpdateLocal()
    {

        base.UpdateLocal();

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
