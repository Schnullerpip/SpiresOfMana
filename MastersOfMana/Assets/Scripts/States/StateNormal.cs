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
}
