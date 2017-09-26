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
	}
	
	// Update is called once per frame
	void Update () {

		//store the input values
		Vector2 input = mRewiredPlayer.GetAxis2D("MoveHorizontal","MoveVertical");
		input *= Time.deltaTime * speed;
	}

	//useful asstes for the PlayerScript

	/*Simple Datacontainer (inner class) for a Pair of Spell and burndown*/
	public struct SpellSlot {
		public A_Spell mSpell;
		public float mBurndown;
	}
}