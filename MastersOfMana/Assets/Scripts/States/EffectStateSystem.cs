using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for the EffectStates
/// </summary>
public class EffectStateSystem
{
    //holds instances of all the possible mStates the player can be in
    public List<A_EffectState> mStates;
    //holds the mCurrent state, the player is in
    public A_EffectState mCurrent;

    /*Each Player will hold his or her specific state instances in a dictionary
     * if a state change occures the 'new' state can be adressed through the player's
     * dictionary with the respective EffectStateID as the key */
    public enum EffectStateID { Normal};



    //Constructor
    public EffectStateSystem(PlayerScript player)
    {
        //instantiate all pissible mStates the player can be in and hold them ready to access
        mStates = new List<A_EffectState>()
        {
            new EffectStateNormal(player),
        };

        mCurrent = mStates[0];
    }

    public A_EffectState GetState(EffectStateID idx)
    {
        return mStates[(int)idx];
    }

    public void SetState(EffectStateID idx)
    {
        mCurrent = mStates[(int)idx];
    }
}