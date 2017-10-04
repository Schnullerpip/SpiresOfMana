using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class, that sets up the minimal needs for a state -> to know a player
/// </summary>
public class A_State
{
    public PlayerScript player;

    public A_State(PlayerScript player)
    {
        this.player = player;
    }

    //prevent standard construction
    private A_State() {}

	// Use this for continuous statechecks
	public virtual void Update () {}

    //statemethods each statesystem will need
    public virtual void Hurt(float amount) { }
}
