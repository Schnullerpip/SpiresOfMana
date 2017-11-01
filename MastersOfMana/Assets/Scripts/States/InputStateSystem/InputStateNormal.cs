using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateNormal : A_InputState
{
    public InputStateNormal(PlayerScript player) : base(player) { }

	public override void Move (Vector2 input)
	{
		base.Move (input);

		Vector3 moveForce = World2DToLocal3D(input, player.transform);

		//override moveForce in player script
		player.movement.SetMoveInput(moveForce);
	}
}