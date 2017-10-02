﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerHealthScript))]
/// <summary>
/// Defines the basic properties for a player
/// </summary>
public class PlayerScript : NetworkBehaviour
{

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
    public A_Spell debug_spell_1;
    public A_Spell debug_spell_2;
    public A_Spell debug_spell_3;

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
	[Tooltip("Degrees per seconds")]
	public float aimSpeed = 360;    
	public float jumpStrength = 5;
	public float minDistanceToGround = 0.1f;
	private bool mIsGrounded = false;

	private Rigidbody rigid;

	public Vector3 moveInputForce;
	public float yAim = 0;

	public Vector3 lookDirection;

	protected Rewired.Player rewiredPlayer;

	public PlayerCamera cameraRig;
	public Transform handTransform;

	void Awake()
	{
		if(cameraRig == null)
		{
			Debug.LogWarning("No camera rig assigned, consider creating one during runtime? Or don't. I'm not your boss. kthx");
		}
	}

	// Use this for initialization
	void Start ()
	{

        //TODO delete this eventually
	    spellSlot_1 = new SpellSlot()
	    {
	        cooldown = 0,
	        spell = debug_spell_1
	    };
	    spellSlot_2 = new SpellSlot()
	    {
	        cooldown = 0,
	        spell = debug_spell_2
	    };
	    spellSlot_3 = new SpellSlot()
	    {
	        cooldown = 0,
	        spell = debug_spell_3
	    };


        //initialize the statesystems
        inputStateSystem = new InputStateSystem(this);
        effectStateSystem = new EffectStateSystem(this);
        cCastStateSystem = new CastStateSystem(this);

        //initialize Inpur handler
	    rewiredPlayer = ReInput.players.GetPlayer(0);

		rigid = GetComponent<Rigidbody>();
	}

    // Use this for initialization on local Player only
    override public void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            cameraRig = Instantiate(cameraRig);
            cameraRig.GetComponent<PlayerCamera>().followTarget = this;
        }
    }

    // Update is called once per frame
    void Update () 
	{
        // Update only on the local player
        if (!isLocalPlayer)
            return;

        //STEP 1 - Decrease the cooldown in the associated spellslots
        DecreaseCooldowns();

        //STEP 2
        //TODO this is not how the spells should be polled!!!!! only for testing!!!! DELETE THIS EVENTUALLY
        if (Input.GetKeyDown("z")) {
            spellSlot_1.Cast(this);
        }
        if (Input.GetKeyDown("u")) {
            spellSlot_2.Cast(this);
        }
        if (Input.GetKeyDown("i")) {
            spellSlot_3.Cast(this);
        }

        //STEP 3

        //STEP n

		//store the input values
		Vector2 movementInput = rewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical");

		//store the aim input, either mouse or right analog stick
		Vector2 aimInput = rewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical");
		//take framerate into consideration
		aimInput *= Time.deltaTime * aimSpeed;

		//rotate the entire player along its y-axis
		transform.Rotate(0,aimInput.x,0);
		//prevent spinning around the z-Axis (no backflips allowed)
		yAim = Mathf.Clamp(yAim + aimInput.y, -89, 89);
		//calculate the lookDirection vector with the current forward vector as a basis, rotating up or down
		lookDirection = Quaternion.AngleAxis(-yAim, transform.right) * transform.forward;

		#if UNITY_EDITOR 
		Debug.DrawRay(transform.position+Vector3.up*1.8f, lookDirection, Color.red);
		#endif

		//propergate various inputs to the statesystems
		#region Input
		if(rewiredPlayer.GetButtonDown("Jump"))
		{
			inputStateSystem.current.Jump();
		}

        inputStateSystem.current.Move(movementInput);

		//TODO: define mouse & keyboard / controller schemes, "CastSpell" not final axis name
		if(rewiredPlayer.GetButtonDown("CastSpell"))
		{
			inputStateSystem.current.Cast_Spell_1();
		}

        inputStateSystem.current.Move(movementInput);
		#endregion
	}

	//TODO: delete this method, testing purpose only
	/// <summary>
	/// !TESTING PURPOSE ONLY! 
	/// This methods creates a GameObject with a LineRenderer to display the line between Hand and Raycast Hit by mouse.
	/// </summary>
	/// <param name="worldSpacePosition">World space position.</param>
	public void DebugRayFromHandToPosition(Vector3 worldSpacePosition)
	{
//		Debug.DrawLine(handTransform.position, worldSpacePosition, Color.yellow, 10);
		GameObject lineGO = new GameObject("line");

		LineRenderer line = lineGO.AddComponent<LineRenderer>();
		line.positionCount = 2;
		line.SetPosition(0,handTransform.position);
		line.SetPosition(1,worldSpacePosition);
		line.widthMultiplier = .1f;

		Destroy(lineGO,10);
	}
		
	void FixedUpdate()
	{
		//move the character
		rigid.MovePosition(rigid.position + (moveInputForce * Time.deltaTime * movementAcceleration));

		//check if the character is on the ground, by raycasting from the feet toward the ground
		mIsGrounded = Physics.Raycast(transform.position, Vector3.down, minDistanceToGround);
	}

	/// <summary>
	/// Let's the character jump with the default jumpStength
	/// </summary>
	public void Jump()
	{
		//TODO grounded
//		rigid.AddForce(Vector3.up*jumpStrength,ForceMode.Impulse);

		Jump(jumpStrength);
	}

	/// <summary>
	/// Let's the character jump with a specified jumpStrength
	/// </summary>
	/// <param name="jumpForce">Jump force.</param>
	public void Jump(float jumpStrength)
	{
		//TODO grounded
		if(mIsGrounded)
		{
			rigid.AddForce(Vector3.up * jumpStrength,ForceMode.VelocityChange);
		}
	}

    private void DecreaseCooldowns()
    {
        if ((spellSlot_1.cooldown -= Time.deltaTime) < 0)
        {
            spellSlot_1.cooldown = 0;
        }
        if ((spellSlot_2.cooldown -= Time.deltaTime) < 0)
        {
            spellSlot_2.cooldown = 0;
        }
        if ((spellSlot_3.cooldown -= Time.deltaTime) < 0)
        {
            spellSlot_3.cooldown = 0;
        }
    }

	//useful asstes for the PlayerScript

	/// <summary>
	/// Simple Datacontainer (inner class) for a Pair of Spell and cooldown
	/// </summary>
	public struct SpellSlot {
		public A_Spell spell;
		public float cooldown;

        /// <summary>
        /// casts the spell inside the slot and also adjusts the cooldown accordingly
        /// This automatically assumes, that the overlaying PlayerScript's update routine decreases the spellslot's cooldown continuously
        /// </summary>
        /// <param name="caster"></param>
	    public void Cast(PlayerScript caster)
	    {
	        if (cooldown <= 0)
	        {
	            cooldown = spell.coolDownInSeconds;
                spell.Cast(caster);
	        }
	    }
	}
}