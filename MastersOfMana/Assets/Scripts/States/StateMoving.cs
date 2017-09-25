using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMoving : A_State
{
    public StateMoving(PlayerScript player) : base(player) { }

	public override void Update ()
	{
		base.Update ();

		//store the input values
		Vector2 input = mRewiredPlayer.GetAxis2D("MoveHorizontal","MoveVertical");

		//if the player didn't give any input
		if(input.sqrMagnitude < float.Epsilon)
		{
			//switch state to normal
			player.mCurrentState = player.mPlayerStates[StateID.Normal];
			return;
		}

		input *= Time.deltaTime * player.speed;

		//cache position
		Vector3 pos = player.transform.position;
		pos.x += input.x;
		pos.z += input.y;
		player.transform.position = pos;
	}
}
