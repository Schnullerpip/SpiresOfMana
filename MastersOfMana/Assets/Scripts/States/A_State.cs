using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class, that sets up the minimal needs for a state -> to know a mPlayer
/// </summary>
public class A_State
{
    private PlayerScript mPlayer;

    public A_State(PlayerScript mPlayer)
    {
        this.mPlayer = mPlayer;
    }

    //prevent standard construction
    private A_State() {}

	// Use this for continuous statechecks
	public virtual void Update () {}
}
