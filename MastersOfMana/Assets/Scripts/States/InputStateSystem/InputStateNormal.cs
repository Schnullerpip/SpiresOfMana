using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateNormal : A_InputState
{
    public InputStateNormal(PlayerScript player) : base(player) { }


    public override void Hurt()
    {
        //get the instance of the hurt state and ask for it in the state dictionary
        mPlayer.mInputStateSystem.SetState(InputStateSystem.InputStateID.Hurt);
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
		    mPlayer.mInputStateSystem.SetState(InputStateSystem.InputStateID.Moving);
            mPlayer.mInputStateSystem.mCurrent.Move(input);
			return;
		}

	}
}
