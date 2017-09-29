using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(Rigidbody))]
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
    

	public float movementAcceleration = 10;    
	public float aimSpeed = 10;    
	public float jumpStrength = 5;

	public float gravityMultiplier = 2;

	private Rigidbody rigid;

	public Vector3 moveInputForce;
	public float yAim = 0;

	public Vector3 lookDirection;

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

		rigid = GetComponent<Rigidbody>();

	}
	// Update is called once per frame
	void Update () 
	{
		//store the input values
		Vector2 movementInput = mRewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical");

		Vector2 aimInput = mRewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical");
		aimInput *= Time.deltaTime * aimSpeed;

		if(mRewiredPlayer.GetButtonDown("Jump"))
		{
			mInputStateSystem.current.Jump();
		}

        mInputStateSystem.current.Move(movementInput);
		mInputStateSystem.current.Aim(aimInput);

		lookDirection = Quaternion.AngleAxis(-yAim, transform.right) * transform.forward;

		Debug.DrawRay(transform.position+Vector3.up*1.8f, lookDirection, Color.red);
	}
		
	void FixedUpdate()
	{
		rigid.MovePosition(rigid.position + moveInputForce * Time.deltaTime * movementAcceleration);
	}

	public void Jump()
	{
		//TODO grounded
		rigid.AddForce(Vector3.up*jumpStrength,ForceMode.Impulse);
	}

	public void test(PlayerScript i)
	{
		
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