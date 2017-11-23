using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for the InputStates
/// </summary>
public class InputStateSystem
{
    //holds instances of all the possible states the player can be in
    public List<A_InputState> states;
    //holds the current state, the player is in
    public A_InputState current;

	/// <summary>
	/// Each Player will hold his or her specific state instances in a dictionary
	/// if a state change occures the 'new' state can be adressed through the player's
	/// dictionary with the respective InputStateID as the key
	/// </summary>
    public enum InputStateID {Normal, Hurt, Idle, Paralyzed};



    //Constructor
    public InputStateSystem(PlayerScript player)
    {
        //instantiate all pissible states the player can be in and hold them ready to access
        states = new List<A_InputState>()
        {
            new InputStateNormal(player),
            new InputStateHurt(player),
            new InputStateIdle(player),
            new InputStateParalyzed(player)
        };

        current = states[0];
    }

    public void UpdateSynchronized()
    {
        current.UpdateSynchronized();
    }
    public void UpdateLocal()
    {
        current.UpdateLocal();
    }

    public A_InputState GetState(InputStateID idx)
    {
        return states[(int)idx];
    }

    public void SetState(InputStateID idx)
    {
        current = states[(int)idx];
        current.Init();
    }
}