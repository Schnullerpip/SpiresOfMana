using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for the CastStates
/// </summary>
public class CastStateSystem
{
    //holds instances of all the possible states the player can be in
    public List<A_CastState> states;
    //holds the current state, the player is in
    public A_CastState current;

    /*Each Player will hold his or her specific state instances in a dictionary
     * if a state change occures the 'new' state can be adressed through the player's
     * dictionary with the respective CastStateID as the key */
    public enum CastStateID { Normal };



    //Constructor
    public CastStateSystem(PlayerScript player)
    {
        //instantiate all pissible states the player can be in and hold them ready to access
        states = new List<A_CastState>()
        {
            new CastStateNormal(player),
        };

        current = states[0];
    }

    public A_CastState GetState(CastStateID idx)
    {
        return states[(int)idx];
    }

    public void SetState(CastStateID idx)
    {
        current = states[(int)idx];
    }
}