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
}
