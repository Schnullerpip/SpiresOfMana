using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateNormal : A_State
{
    public StateNormal(PlayerScript player) : base(player) { }


    public override void Hurt()
    {
        //get the instance of the hurt state and ask for it in the state dictionary
        player.mCurrentState = player.mPlayerStates[StateID.Hurt];
    }

	public override void Update ()
	{
		base.Update ();
	}

	public override void Move (Vector2 input)
	{
		base.Move (input);
		if(input.sqrMagnitude > float.Epsilon)
		{
			player.mCurrentState = player.mPlayerStates[StateID.Moving];
			player.mCurrentState.Move(input);
			return;
		}
	}
}
