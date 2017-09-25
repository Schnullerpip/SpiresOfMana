using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    //member
    public A_State mCurrentState;
    public Dictionary<A_State.StateID, A_State> mPlayerStates;

	public float speed = 10;
    

	// Use this for initialization
	void Start () {
        //instantiate all pissible states the player can be in and hold them ready to access
        mPlayerStates = new Dictionary<A_State.StateID, A_State>();
        mPlayerStates.Add(A_State.StateID.Normal, new StateNormal(this));
        mPlayerStates.Add(A_State.StateID.Moving, new StateMoving(this));
        mPlayerStates.Add(A_State.StateID.Hurt, new StateHurt(this));

		mCurrentState = mPlayerStates[A_State.StateID.Normal];
	}
	
	// Update is called once per frame
	void Update () {
		mCurrentState.Update();
	}
}
