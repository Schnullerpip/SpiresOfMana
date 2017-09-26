using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for the InputStates
/// </summary>
public class InputStateSystem
{
    //holds instances of all the possible mStates the player can be in
    public List<A_InputState> mStates;
    //holds the mCurrent state, the player is in
    public A_InputState mCurrent;

    /*Each Player will hold his or her specific state instances in a dictionary
     * if a state change occures the 'new' state can be adressed through the player's
     * dictionary with the respective InputStateID as the key */
    public enum InputStateID {Normal, Moving, Hurt};



    //Constructor
    public InputStateSystem(PlayerScript player)
    {
        //instantiate all pissible mStates the player can be in and hold them ready to access
        mStates = new List<A_InputState>()
        {
            new InputStateNormal(player),
            new InputStateMoving(player),
            new InputStateHurt(player),
        };

        mCurrent = mStates[0];
    }

    public A_InputState GetState(InputStateID idx)
    {
        return mStates[(int)idx];
    }

    public void SetState(InputStateID idx)
    {
        mCurrent = mStates[(int)idx];
    }
}