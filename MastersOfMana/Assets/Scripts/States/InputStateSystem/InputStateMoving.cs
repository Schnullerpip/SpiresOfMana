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
		    player.inputStateSystem.SetState(InputStateSystem.InputStateID.Normal);
			player.inputStateSystem.current.Move(input);
			player.moveInputForce = Vector3.zero;
			return;
		}
			
		Vector3 moveForce = new Vector3(input.x, 0, input.y);
		//from global to local space
		moveForce = player.transform.TransformDirection(moveForce);

		//override moveForce in player script
		player.moveInputForce = moveForce;
	}
}
