﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    //member
    //holds the current state, the player is in
    public A_State mCurrentState;
    //holds instances of all the possible states the player can be in
    public Dictionary<A_State.StateID, A_State> mPlayerStates;
    

	// Use this for initialization
	void Start () {
        //instantiate all pissible states the player can be in and hold them ready to access
        mPlayerStates = new Dictionary<A_State.StateID, A_State>();
        mPlayerStates.Add(A_State.StateID.Normal, new StateNormal(this));
        mPlayerStates.Add(A_State.StateID.Moving, new StateMoving(this));
        mPlayerStates.Add(A_State.StateID.Hurt, new StateHurt(this));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
