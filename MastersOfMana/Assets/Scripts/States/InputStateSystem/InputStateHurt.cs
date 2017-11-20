using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateHurt : A_InputState
{
    public InputStateHurt(PlayerScript player) : base(player) { }

	public override void Move (Vector2 input)
	{
		base.Move (input);

		Vector3 moveForce = World2DToLocal3D (input, player.transform);

		//override moveForce in player script
		player.movement.SetMoveInputHurt(moveForce);
	}
}
