using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_State {

    //static information
    public static int
        StateIDNormal = 0,
        StateIDMoving = 1,
        StateIDHurt = 2;

    public enum StateID { Normal, Moving, Hurt};

    //shared datafields
    public PlayerScript player;


    //Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="A_State"/> class.
	/// </summary>
	/// <param name="player">Player.</param>
    public A_State(PlayerScript player) {
        this.player = player;
    }

    //behaviour distinction
    public virtual void Hurt() { }
    public virtual void Move(Vector2 input) { }
    public virtual void Jump() { }
    public virtual void Cast_Spell_1() { }
    public virtual void Cast_Spell_2() { }
    public virtual void Cast_Spell_3() { }

	// Use this for continuous statechecks
	public virtual void Update () {}
}
