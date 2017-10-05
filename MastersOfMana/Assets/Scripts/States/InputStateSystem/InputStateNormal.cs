using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateNormal : A_InputState
{
    public InputStateNormal(PlayerScript player) : base(player) { }


    public override float Hurt(float amount)
    {
        //get the instance of the hurt state and ask for it in the state dictionary
        player.inputStateSystem.SetState(InputStateSystem.InputStateID.Hurt);
        return amount;
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
		    player.inputStateSystem.SetState(InputStateSystem.InputStateID.Moving);
            player.inputStateSystem.current.Move(input);
			return;
		}

	}
}