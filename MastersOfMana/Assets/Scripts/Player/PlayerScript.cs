using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(CharacterController))]
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
    

	public float movementSpeed = 10;    
	public float aimSpeed = 10;    
	public CharacterController character;

	public Vector3 moveForce;

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

		character = GetComponent<CharacterController>();
	}

    // Update is called once per frame
	void Update () {

		//store the input values
		Vector2 movementInput = mRewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical");
		movementInput *= Time.deltaTime * movementSpeed;

		Vector2 aimInput = mRewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical");
		aimInput *= Time.deltaTime * aimSpeed;

        mInputStateSystem.mCurrent.Move(movementInput);
		mInputStateSystem.mCurrent.Aim(aimInput);

		//apply the accumulated force, in addition to gravity
		character.Move(moveForce + Physics.gravity * Time.deltaTime);
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