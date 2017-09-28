using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerHealthScript))]
/// <summary>
/// Defines the basic properties for a player
/// </summary>
public class PlayerScript : MonoBehaviour {

    //member
    public InputStateSystem inputStateSystem;
    public EffectStateSystem effectStateSystem;
    public CastStateSystem cCastStateSystem;

    //spellslots
    public SpellSlot
        spellSlot_1,
        spellSlot_2,
        spellSlot_3;
    

	public float movementAcceleration = 10;    
	public float aimSpeed = 10;    
	public float jumpStrength = 5;
	private Rigidbody rigid;

	public Vector3 moveInputForce;

	protected Rewired.Player mRewiredPlayer;

	// Use this for initialization
	void Start ()
	{
        //initialize the statesystems
        inputStateSystem = new InputStateSystem(this);
        effectStateSystem = new EffectStateSystem(this);
        cCastStateSystem = new CastStateSystem(this);

        //initialize Inpur handler
	    mRewiredPlayer = ReInput.players.GetPlayer(0);

		rigid = GetComponent<Rigidbody>();

	}
	// Update is called once per frame
	void Update () 
	{
		//store the input values
		Vector2 movementInput = mRewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical");
		movementInput *= Time.deltaTime * movementAcceleration;

		Vector2 aimInput = mRewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical");
		aimInput *= Time.deltaTime * aimSpeed;

		if(mRewiredPlayer.GetButtonDown("Jump"))
		{
			inputStateSystem.current.Jump();
		}

        inputStateSystem.current.Move(movementInput);
		inputStateSystem.current.Aim(aimInput);

	}
		
	void FixedUpdate()
	{
		rigid.MovePosition(rigid.position + moveInputForce);
	}

	public void Jump()
	{
		//TODO grounded
		rigid.AddForce(Vector3.up*jumpStrength,ForceMode.Impulse);
	}

	//useful asstes for the PlayerScript

	/// <summary>
	/// Simple Datacontainer (inner class) for a Pair of Spell and cooldown
	/// </summary>
	public struct SpellSlot {
		public A_Spell mSpell;
		public float mCooldown;
	}
}