﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
			return;
		}

        //cache position
        Vector3 pos = mPlayer.transform.position;
		pos.x += input.x;
		pos.z += input.y;
		mPlayer.transform.position = pos;
	}
}
