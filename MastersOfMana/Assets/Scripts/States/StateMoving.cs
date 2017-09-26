using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMoving : A_State
{
    public StateMoving(PlayerScript player) : base(player) { }

	public override void Update ()
	{
		base.Update ();
	}

	public override void Move (Vector2 input)
	{
		base.Move (input);

		if(input.sqrMagnitude <= float.Epsilon)
		{
			player.mCurrentState = player.mPlayerStates[StateID.Normal];
			player.mCurrentState.Move(input);
			return;
		}

		//cache position
		Vector3 pos = player.transform.position;
		pos.x += input.x;
		pos.z += input.y;
		player.transform.position = pos;
	}
}
