using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// Defines the basic properties for a player
/// </summary>
public class PlayerScript : MonoBehaviour {

    //member
    public InputStateSystem mInputStateSystem;
    public EffectStateSystem mEffectStateSystem;
    public CastStateSystem mCastStateSystem;

    //spellslots
    public SpellSlot
        mSpellSlot_1,
        mSpellSlot_2,
        mSpellSlot_3;
    

	public float speed = 10;    
	
	protected Rewired.Player mRewiredPlayer;

	// Use this for initialization
	void Start () {
<<<<<<< HEAD
=======
		mRewiredPlayer = ReInput.players.GetPlayer(0);

        //instantiate all pissible states the player can be in and hold them ready to access
        mPlayerStates = new Dictionary<A_State.StateID, A_State>();
        mPlayerStates.Add(A_State.StateID.Normal, new StateNormal(this));
        mPlayerStates.Add(A_State.StateID.Moving, new StateMoving(this));
        mPlayerStates.Add(A_State.StateID.Hurt, new StateHurt(this));

        //
	    mCurrentState = mPlayerStates[A_State.StateID.Normal];
>>>>>>> 6b64c8611b7c834e005d2d34557aa1c407d21a78
	}
	
	// Update is called once per frame
	void Update () {
		mCurrentState.Update();

		//store the input values
		Vector2 input = mRewiredPlayer.GetAxis2D("MoveHorizontal","MoveVertical");
		input *= Time.deltaTime * speed;

		mCurrentState.Move(input);
	}

	//useful asstes for the PlayerScript

	/*Simple Datacontainer (inner class) for a Pair of Spell and burndown*/
	public struct SpellSlot {
		public A_Spell mSpell;
		public float mBurndown;
	}
}