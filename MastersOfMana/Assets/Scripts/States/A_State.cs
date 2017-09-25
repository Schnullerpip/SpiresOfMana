using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class A_State {

    //static information
    public static int
        StateIDNormal = 0,
        StateIDMoving = 1,
        StateIDHurt = 2;

    public enum StateID { Normal, Moving, Hurt};

    //shared datafields
    public PlayerScript player;

	protected Rewired.Player mRewiredPlayer;

    //Constructor
    public A_State(PlayerScript player) {
        this.player = player;
		mRewiredPlayer = ReInput.players.GetPlayer(0);
    }

    //behaviour distinction
    public virtual void Hurt() { }
    public virtual void Move() { }
    public virtual void Jump() { }
    public virtual void Cast_Spell_1() { }
    public virtual void Cast_Spell_2() { }
    public virtual void Cast_Spell_3() { }

	// Use this for continuous statechecks
	public virtual void Update () {}
}
