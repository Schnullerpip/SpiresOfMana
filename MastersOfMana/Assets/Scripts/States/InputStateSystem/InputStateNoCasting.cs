using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateNoCasting : A_InputState {

    public InputStateNoCasting(PlayerScript player) : base(player) { }

    public override void UpdateLocal()
    {
        base.UpdateLocal();

        //store the input values
        Vector2 movementInput = playerInput.GetAxis2D("MoveHorizontal", "MoveVertical");
		Move(movementInput);


		if(playerInput.GetButtonDown("ShoulderSwap"))
		{
			player.aim.GetCameraRig().SwapShoulder();
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