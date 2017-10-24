using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for the EffectStates
/// </summary>
public class EffectStateSystem
{
    //holds instances of all the possible states the player can be in
    public List<A_EffectState> states;
    //holds the current state, the player is in
    public A_EffectState current;

    /*Each Player will hold his or her specific state instances in a dictionary
     * if a state change occures the 'new' state can be adressed through the player's
     * dictionary with the respective EffectStateID as the key */
    public enum EffectStateID { Normal, NoFallDamage, Invincible};



    //Constructor
    public EffectStateSystem(PlayerScript player)
    {
        //instantiate all pissible states the player can be in and hold them ready to access
        states = new List<A_EffectState>()
        {
            new EffectStateNormal(player),
            new EffectStateNoFallDamage(player),
            new EffectStateInvincible(player),
        };

        current = states[0];
    }

    public A_EffectState GetState(EffectStateID idx)
    {
        return states[(int)idx];
    }

    public void SetState(EffectStateID idx)
    {
        current = states[(int)idx];
        current.Init();
    }
}