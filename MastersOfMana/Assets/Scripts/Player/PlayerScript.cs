using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// Defines the basic properties for a player
/// </summary>
[RequireComponent(typeof(PlayerHealthScript))]
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
	void Start ()
	{
        //initialize the statesystems
        mInputStateSystem = new InputStateSystem(this);
        mEffectStateSystem = new EffectStateSystem(this);
        mCastStateSystem = new CastStateSystem(this);

        //initialize Inpur handler
	    mRewiredPlayer = ReInput.players.GetPlayer(0);
	}

    //cached Vector2 forplayer Input
    private Vector2 input;
    // Update is called once per frame
	void Update () {

		//store the input values
		input = mRewiredPlayer.GetAxis2D("MoveHorizontal","MoveVertical");
		input *= Time.deltaTime * speed;

        //forward the player input to the InputStateSystem
        mInputStateSystem.mCurrent.Move(input);
	}

	//useful asstes for the PlayerScript

    /// <summary>
	/// Simple Datacontainer (inner class) for a Pair of Spell and burndown
    /// </summary>
	public struct SpellSlot {
		public A_Spell mSpell;
		public float mBurndown;
	}
}