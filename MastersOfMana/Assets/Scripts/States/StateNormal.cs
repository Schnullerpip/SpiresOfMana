using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateNormal : A_State
{
    public StateNormal(PlayerScript player) : base(player) { }


    public override void Hurt()
    {
        player.mCurrentState = player.mPlayerStates[StateID.Hurt];
    }

	public override void Update ()
	{
		base.Update ();

		//check for any kind of input
		if(mRewiredPlayer.GetAxis2D("MoveHorizontal","MoveVertical").sqrMagnitude > float.Epsilon)
		{
			player.mCurrentState = player.mPlayerStates[StateID.Moving];
			player.mCurrentState.Update();
		}
	}
}
