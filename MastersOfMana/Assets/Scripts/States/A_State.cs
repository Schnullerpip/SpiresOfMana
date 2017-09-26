using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// abstract State, that defines default behaviour for substates and provides the base constructor
/// </summary>
public abstract class A_State {
    //shared datafields
    public PlayerScript player;

    /*Each Player will hold his or her specific state instances in a dictionary
     * if a state change occures the 'new' state can be adressed through the player's
     * dictionary with the respective StateID as the key */
    public enum StateID { Normal, Moving, Hurt};


    //Constructor - this abstract class defines the constructor for all the subclasses
    public A_State(PlayerScript player) {
        this.player = player;
    }
    private A_State() { }

    /*behaviour distinction
     * those are keptempty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */
    public virtual void Hurt() { }
    public virtual void Move() { }
    public virtual void Jump() { }
    public virtual void Cast_Spell_1() { }
    public virtual void Cast_Spell_2() { }
    public virtual void Cast_Spell_3() { }
    public virtual void Collide() { }


	// Use this for continuous statechecks
	public virtual void Update () {}
}
