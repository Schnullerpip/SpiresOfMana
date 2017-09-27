using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMoving : A_InputState
{
    public InputStateMoving(PlayerScript player) : base(player) { }

	public override void Move (Vector2 input)
	{
		base.Move (input);

		if(input.sqrMagnitude <= float.Epsilon)
		{
		    mPlayer.mInputStateSystem.SetState(InputStateSystem.InputStateID.Normal);
			mPlayer.mInputStateSystem.mCurrent.Move(input);
			mPlayer.moveInputForce = Vector3.zero;
			return;
		}
			
		Vector3 moveForce = new Vector3(input.x, 0, input.y);
		//from global to local space
		moveForce = mPlayer.transform.TransformDirection(moveForce);

		//override moveForce in player script
		mPlayer.moveInputForce = moveForce;
	}
}
