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

    //TODO delete this eventually
    public A_Spell debug_spell;


    /// <summary>
    /// holds references to all the coroutines a spell is running, so they can bes stopped/interrupted when a player is for example hit
    /// and can therefore not continue to cast the spell
    /// </summary>
    public List<IEnumerator> spellRoutines = new List<IEnumerator>();

    public void EnlistCoroutine(IEnumerator spellRoutine)
    {
        spellRoutines.Add(spellRoutine);
    }

	public float movementAcceleration = 10;    
	public float aimSpeed = 10;    
	public float jumpStrength = 5;
	private Rigidbody rigid;

	public Vector3 moveInputForce;

	protected Rewired.Player rewiredPlayer;

	// Use this for initialization
	void Start ()
	{
        //initialize the statesystems
        inputStateSystem = new InputStateSystem(this);
        effectStateSystem = new EffectStateSystem(this);
        cCastStateSystem = new CastStateSystem(this);

        //initialize Inpur handler
	    rewiredPlayer = ReInput.players.GetPlayer(0);

		rigid = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update () 
	{

        //TODO delete this evetually
        debug_spell.Cast(this);

		//store the input values
		Vector2 movementInput = rewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical");
		movementInput *= Time.deltaTime * movementAcceleration;

		Vector2 aimInput = rewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical");
		aimInput *= Time.deltaTime * aimSpeed;

		if(rewiredPlayer.GetButtonDown("Jump"))
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