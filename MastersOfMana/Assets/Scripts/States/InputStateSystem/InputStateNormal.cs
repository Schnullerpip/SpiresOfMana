using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateNormal : A_InputState
{
    public InputStateNormal(PlayerScript player) : base(player) { }


    public override float CalculateDamage(float amount)
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

		Vector3 moveForce = new Vector3(input.x, 0, input.y);

		//from global to local space
		moveForce = player.transform.TransformDirection(moveForce);

		//override moveForce in player script
		player.movement.SetMoveInput(moveForce);
	}
}