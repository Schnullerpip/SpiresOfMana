using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateDead : A_InputState
{
    public InputStateDead(PlayerScript player) : base(player) { }

    public override void Init()
    {
        base.Init();
        //zero out the input so the player doesnt continue moving when dead
        player.movement.SetMoveInput(Vector3.zero);
    }

	public override void Move (Vector2 input)
	{
	}
}
