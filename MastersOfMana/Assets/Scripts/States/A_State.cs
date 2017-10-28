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

    /// <summary>
    /// The function that should be called after a statechange - so the 'new' state can re/set some properties - though few states really need to have this probably
    /// </summary>
    public virtual void Init() { }

    //prevent standard construction
    private A_State() {}

	// Use this for continuous statechecks
	public virtual void Update () {}

    //statemethods each statesystem will need

    /// <summary>
    /// Handles internal statechanges in the case of "I am Taking damage right now!"
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>
    /// the amount of damage, that will be subtracted from the current health
    /// the state may decide or have an effect on how much damage is dealt in this situation,
    /// but it may never actually touch the health field. This is the responsibility of the PlayerHealthScript
    /// </returns>
    public virtual float CalculateDamage(float amount)
    {
        return amount;
    }

    /// <summary>
    /// Handles internal statechanges in the case of 'i am taking falldamage right now'
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public virtual float CalculateFallDamage(float amount)
    {
        return amount;
    }
}
