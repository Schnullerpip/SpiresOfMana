﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Networking;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
/// <summary>
/// Defines the basic properties for a player
/// </summary>
public class PlayerScript : NetworkBehaviour {

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

    public GameObject cameraRig;
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

    override public void OnStartLocalPlayer()
    {
        if(isLocalPlayer)
        {
            cameraRig = Instantiate(cameraRig);
            cameraRig.GetComponent<SmoothFollow>().followTarget = transform;
        }
    }

	// Update is called once per frame
    void Update () {

        if (!isLocalPlayer)
            return;

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

	/*Simple Datacontainer (inner class) for a Pair of Spell and burndown*/
	public struct SpellSlot {
		public A_Spell mSpell;
		public float mBurndown;
	}
}