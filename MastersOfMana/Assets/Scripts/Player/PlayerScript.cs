using System;
using System.Collections;
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

    /// <summary>
    /// holds references to all the coroutines a spell is running, so they can bes stopped/interrupted w4hen a player is for example hit
    /// and can therefore not continue to cast the spell
    /// </summary>
    public List<IEnumerator> spellRoutines = new List<IEnumerator>();

    public void EnlistCoroutine(IEnumerator spellRoutine)
    {
        spellRoutines.Add(spellRoutine);
    }

	[Header("Movement")][SyncVar]
	public float speed = 4;  
	[Range(0,1)]
	public float focusSpeedSlowdown = .25f;
    [SyncVar]
	public float jumpStrength = 5;
	public float fallGravityMultiplier = 1.2f;
	[Tooltip("How much slower is the player when he/she walks backwards? 0 = no slowdown, 1 = fullstop")]
	[Range(0.0f,1.0f)]
	public float amountOfReverseSlowdown = 0.0f;
	[Tooltip("At which angle does the player still move with fullspeed?")]
	[Range(0.0f,180.0f)]
	public int maxFullspeedAngle = 90;

	[HideInInspector]
	public Vector3 moveInputForce;
	public FeetGroundCheck feet;

	[Header("Aim")]
	[Tooltip("Degrees per seconds")]
	public float aimSpeed = 360;    
	public float aimAssistInUnits = 1.0f;
	[Tooltip("How fast is the aim when focused? 0 = freeze, 1 = no slowdown")]
	[Range(0,1)]
	public float focusAimSpeedFactor = .25f;
	[Tooltip("How many units can the player move the cursor when locked on?")]
	public float maxAimRefinementMagnitude = 1f;
	[HideInInspector]
	public float yAim = 0;
	[HideInInspector][SyncVar]
	public Vector3 lookDirection;
	public PlayerCamera cameraRig;
	public Transform handTransform;
	private Vector3 mAimRefinement;

	private bool mFocusActive = false;
	private Rigidbody mRigidbody;
	private Collider mFocusedTarget = null;
	protected Rewired.Player rewiredPlayer;

	void Awake()
	{
		lookDirection = transform.forward;

		if(cameraRig == null)
		{
			Debug.LogWarning("No camera rig assigned, consider creating one during runtime? Or don't. I'm not your boss. kthx");
		}
//		Cursor.lockState = CursorLockMode.Locked;
	}

	// Use this for initialization
	void Start ()
	{
        //initialize the statesystems
        inputStateSystem = new InputStateSystem(this);
        effectStateSystem = new EffectStateSystem(this);
        cCastStateSystem = new CastStateSystem(this);

        //initialize Inpur handler
	    rewiredPlayer = ReInput.players.GetPlayer(0);

		mRigidbody = GetComponent<Rigidbody>();
	}

    [Command]
    private void CmdGiveGo()
    {
        GameManager.Go();
    }

    // Use this for initialization on local Player only
    override public void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            CmdGiveGo();
            cameraRig = Instantiate(cameraRig);
            cameraRig.GetComponent<PlayerCamera>().followTarget = this;
        }
    }

    /// <summary>
    /// the direction the player aims during a cast (this field only is valid, during a cast routine, on the server!
    /// </summary>
    private Vector3 mAimDirection;
    public Vector3 GetAimDirection()
    {
        return mAimDirection;
    }
    /// <summary>
    /// Calculates the aim direction for a player considering its camerarig, that is only o the local player! The resulting Vector3 can 
    /// be passed to the spell Commands, so the server can update its aiming direction, the moment it is supposed to cast a spell
    /// </summary>
    private Vector3 CalculateAimDirection()
    {
        RaycastHit hit;
        return cameraRig.CenterRaycast(out hit) ? Vector3.Normalize(hit.point - handTransform.position) : lookDirection;
    }

    public void CastCmdSpellslot_1()
    {
        CmdSpellslot_1(CalculateAimDirection());
    }
    public void CastCmdSpellslot_2()
    {
        CmdSpellslot_2(CalculateAimDirection());
    }
    public void CastCmdSpellslot_3()
    {
        CmdSpellslot_3(CalculateAimDirection());
    }

    [Command]
    public void CmdSpellslot_1(Vector3 castDirection)
    {
        mAimDirection = castDirection; //update the aimdirection
        spellSlot_1.Cast(this);
    }
    [Command]
    public void CmdSpellslot_2(Vector3 castDirection)
    {
        mAimDirection = castDirection;//update the aimdirection
        spellSlot_2.Cast(this);
    }
    [Command]
    public void CmdSpellslot_3(Vector3 castDirection)
    {
        mAimDirection = castDirection;//update the aimdirection
        spellSlot_3.Cast(this);
    }

    // Update is called once per frame
    void Update () 
	{
        //To be run on the server
        //STEP 1 - Decrease the cooldown in the associated spellslots
        DecreaseCooldowns();

        // Update only on the local player
	    if (!isLocalPlayer)
	    {
            return;
	    }

        //To be run on the clients

        //STEP 2
        //TODO this is not how the spells should be polled!!!!! only for testing!!!! DELETE THIS EVENTUALLY
        if (rewiredPlayer.GetButtonDown("CastSpell1")) {
            CastCmdSpellslot_1();
        }
		if (rewiredPlayer.GetButtonDown("CastSpell2")) {
            CastCmdSpellslot_2();
        }
		if (rewiredPlayer.GetButtonDown("CastSpell3")) {
            CastCmdSpellslot_3();
        }

        //STEP 3

        //STEP n

		//store the input values
		Vector2 movementInput = rewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical");
		movementInput = Vector3.ClampMagnitude(movementInput,1);

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

		if(rewiredPlayer.GetButtonDown("Focus"))
		{
			inputStateSystem.current.StartFocus();
		}

		if(rewiredPlayer.GetButtonUp("Focus"))
		{
			inputStateSystem.current.StopFocus();
		}

		//store the aim input, either mouse or right analog stick
		Vector2 aimInput = rewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical");
		aimInput = Vector3.ClampMagnitude(aimInput,1);

		//take framerate into consideration
		aimInput *= Time.deltaTime * aimSpeed * (mFocusActive ? focusAimSpeedFactor : 1);

		inputStateSystem.current.Aim(aimInput);

		#endregion

		lookDirection = Quaternion.AngleAxis(-yAim, transform.right) * transform.forward;

		#if UNITY_EDITOR 
		Debug.DrawRay(transform.position+Vector3.up*1.8f, lookDirection, Color.red);
		#endif
 	}

	/// <summary>
	/// Determines whether this instance has a focus target.
	/// </summary>
	/// <returns><c>true</c> if this instance has focus target; otherwise, <c>false</c>.</returns>
	public bool HasFocusTarget()
	{
		return mFocusedTarget != null;
	}

	/// <summary>
	/// Validates if the focus target is in view. Sets the FocusTarget to null if not in free view.
	/// </summary>
	/// <returns><c>true</c>, if focus target view was validated, <c>false</c> otherwise.</returns>
	public bool ValidateFocusTargetView ()
	{
		if(mFocusedTarget == null)
		{
			return false;
		}
		RaycastHit hit;
		//check if the focus target is still in view
		cameraRig.RaycastCheck (mFocusedTarget.bounds.center, out hit);
		if (hit.collider.gameObject != mFocusedTarget.gameObject) 
		{
			mFocusedTarget = null;
			return false;
		}
		return true;
	}

	/// <summary>
	/// Aims the player by the specified aimMovement.
	/// </summary>
	/// <param name="aimMovement">Aim movement.</param>
	public void Aim (Vector2 aimMovement)
	{
		//rotate the entire player along its y-axis
		transform.Rotate (0, aimMovement.x, 0);
		//prevent spinning around the z-Axis (no backflips allowed)
		yAim = Mathf.Clamp (yAim + aimMovement.y, -89, 89);
	}

	/// <summary>
	/// Refines the aim.
	/// </summary>
	/// <param name="aimInput">Aim input.</param>
	public void RefineAim (Vector2 aimInput)
	{
		//convert from local to worldspace and reduces the input by pi/2 since it was parameterized for angular movement of player
		mAimRefinement += transform.TransformDirection (aimInput) / Mathf.PI * .5f;
		//clamp to 
		mAimRefinement = Vector3.ClampMagnitude (mAimRefinement, maxAimRefinementMagnitude);

		//get the current controller
		Rewired.Controller lastController = rewiredPlayer.controllers.GetLastActiveController();
		if(lastController != null && lastController.type == ControllerType.Joystick)
		{
			//do this snapback only for joystick controls
			if (aimInput.sqrMagnitude <= 0) 
			{
				mAimRefinement = Vector3.MoveTowards (mAimRefinement, Vector3.zero, Time.deltaTime * aimSpeed * Time.deltaTime);
			}
		}
	}

	public void ResetRefinement()
	{
		mAimRefinement = Vector3.zero;
	}

	/// <summary>
	/// Rotates the player towards the focus target.
	/// </summary>
	public void RotateTowardsFocusTarget ()
	{
		//get the direction to the target, from the actual cameras position
		Vector3 dirToTarget = (mFocusedTarget.bounds.center + mAimRefinement) - cameraRig.GetCamera().transform.position;
		//rotate towards the target
		float yRotation = Mathf.Atan2 (dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis (yRotation, Vector3.up);
		yAim = -Vector3.SignedAngle (transform.forward, dirToTarget, transform.right);
	}

	/// <summary>
	/// Determines whether the player is focused.
	/// </summary>
	/// <returns><c>true</c> if this instance is focused; otherwise, <c>false</c>.</returns>
	public bool IsFocused()
	{
		return mFocusActive;
	}

	/// <summary>
	/// Starts the focus. Sets FocusActive to true and looks for a potential FocusTarget. Does not guarantee a FocusTarget.
	/// </summary>
	public void StartFocus()
	{
		mFocusActive = true;
		mFocusedTarget = FocusAssistTarget(aimAssistInUnits);
	}

	/// <summary>
	/// Stops the focus. Sets FocusActive to false and the FocusTarget to null.
	/// </summary>
	public void StopFocus()
	{
		mFocusActive = false;
		mFocusedTarget = null;
	}

	/// <summary>
	/// Returns a Collider thats within the specified 
	/// maxAngle of the camera and that is not obstructed.	/// </summary>
	/// <returns>The assist target.</returns>
	/// <param name="maxUnitsOff">Max units off.</param>
	private Collider FocusAssistTarget(float maxUnitsOff)
	{
		//shoot a raycast in the middle of the screen
		RaycastHit hit;
		if(cameraRig.CenterRaycast(out hit))
		{
			//if we hit a healthscript, take that as our aim assist target
			HealthScript h = hit.collider.GetComponentInParent<HealthScript>();
			if(h != null)
			{
				return hit.collider;
			}
		}

		//TODO: cache this, maybe gamemanager?
		List<HealthScript> allHealthScripts = new List<HealthScript>(GameObject.FindObjectsOfType<HealthScript>());

//		//sort by distance 
//		allHealthScripts.Sort(
//			delegate(HealthScript a, HealthScript b) 
//			{
//				return Vector3.Distance(transform.position,a.transform.position).CompareTo(Vector3.Distance(transform.position,b.transform.position));
//			}
//		);

		//sort by angle
		allHealthScripts.Sort(
			delegate(HealthScript a, HealthScript b) 
			{
				return Vector3.Angle(lookDirection, a.transform.position - cameraRig.GetCamera().transform.position)
					.CompareTo(Vector3.Angle(lookDirection, b.transform.position - cameraRig.GetCamera().transform.position));
			}
		);
			
		//iterate through all healthscripts
		foreach (HealthScript aHealthScript in allHealthScripts) 
		{
			//skip player him/herself
			if(aHealthScript.gameObject == this.gameObject)
			{
				continue;
			}
			
			//skip if the target is behind the player
			if(transform.InverseTransformPoint(aHealthScript.transform.position).z < 0)
			{
				continue;
			}

			Collider healthScriptCollider = aHealthScript.GetComponentInChildren<Collider>();

			//get the vector to the target, from the position of the camera
			Vector3 dirToTarget = healthScriptCollider.bounds.center - cameraRig.GetCamera().transform.position;

			//priject the direction vector to the target onto a plane, defined by the lookDirection
			//this way it acts as if the target hat a kind of 2d hitbox (circle) that expands maxUnitsOff into every direciton 
			if(Vector3.ProjectOnPlane(dirToTarget, - lookDirection).sqrMagnitude < maxUnitsOff * maxUnitsOff)
			{
				if(cameraRig.RaycastCheck(healthScriptCollider.bounds.center, out hit))
				{
					//TODO find a better method to varify target, perhabs tags?
					if(hit.collider.GetComponentInParent<HealthScript>() == aHealthScript)
					{
						return hit.collider;
					}
				}
			}
		}

		//nothing found
		return null;
	}

	//TODO: delete this method, testing purpose only
	/// <summary>
	/// !TESTING PURPOSE ONLY! 
	/// This methods creates a GameObject with a LineRenderer to display the line between Hand and Raycast Hit by mouse.
	/// </summary>
	/// <SpellslotLambda name="worldSpacePosition">World space position.</SpellslotLambda>
	public void DebugRayFromHandToPosition(Vector3 worldSpacePosition)
	{
		GameObject lineGO = new GameObject("line");

		LineRenderer line = lineGO.AddComponent<LineRenderer>();
		line.positionCount = 2;
		line.SetPosition(0,handTransform.position);
		line.SetPosition(1,worldSpacePosition);
		line.widthMultiplier = .1f;

		Destroy(lineGO,10);
	}


	void OnCollisionStay(Collision collisionInfo)
	{
		foreach (ContactPoint contact in collisionInfo.contacts) 
		{
			if(contact.thisCollider == feet.collider)
			{
				feet.Collision(contact);
			}
		}
	}
		
	void FixedUpdate()
	{
		feet.PhysicsUpdate();

		if(feet.IsGrounded())
		{
			Debug.DrawRay(transform.position, feet.GetGroundNormal(), (feet.currentSlopeAngle < feet.maxSlope ? Color.white : Color.magenta), 10);
		}

		Vector3 direction = moveInputForce * Time.deltaTime * speed * (mFocusActive ? focusSpeedSlowdown : 1);
		Vector2 directionXZ = direction.xz();

		//calculate the amount of slowdown, by comparing the direction with the forward vector of the character
		Vector2 forwardXZ = transform.forward.xz();
		//value between 0 and 1, 1 being total reversaldamping, 0 being no damping
		float reverseDamping = Mathf.Clamp01((Vector2.Angle (forwardXZ, directionXZ) - maxFullspeedAngle) / 180 * 2);
		reverseDamping *= amountOfReverseSlowdown;
		direction *= 1 - reverseDamping;

		//increase the falling speed to make it feel a bit less floaty
		if(mRigidbody.velocity.y < 0)
		{
			mRigidbody.velocity += Physics.gravity * fallGravityMultiplier * Time.deltaTime;
		}

		//move the character
		mRigidbody.MovePosition(mRigidbody.position + direction);

		Debug.DrawRay(transform.position+Vector3.up, moveInputForce, Color.cyan);
		Debug.DrawRay(transform.position+Vector3.up, mRigidbody.velocity, Color.green);

		//if the player gets input
		if(direction.sqrMagnitude > 0)
		{
			//calculate the angle between the movemement and external forces
			float angle = Vector2.Angle(mRigidbody.velocity.xz(),directionXZ);
			//move the rigidbody's velocity towards zero in the xz plane, proportional to the angle
			mRigidbody.velocity = Vector3.MoveTowards(mRigidbody.velocity, new Vector3(0,mRigidbody.velocity.y,0), speed * Time.deltaTime * angle / 180);
		}
	}

	/// <summary>
	/// Let's the character jump with the default jumpStength
	/// </summary>
	public void Jump()
	{
		Jump(jumpStrength);
	}

	/// <summary>
	/// Let's the character jump with a specified jumpStrength
	/// </summary>
	/// <SpellslotLambda name="jumpForce">Jump force.</SpellslotLambda>
	public void Jump(float jumpStrength)
	{
		if(feet.IsGrounded())
		{
			mRigidbody.AddForce(Vector3.up * jumpStrength,ForceMode.VelocityChange);
		}
	}

    /// <summary>
    /// Is supposed to be placed inside the update loop of the player, yet only to be executed on the server
    /// </summary>
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

    //Remote Procedure Calls!
    [ClientRpc]
    public void RpcChangeInputState(InputStateSystem.InputStateID newStateID)
    {
        inputStateSystem.SetState(newStateID);
    }
		
	//useful asstes for the PlayerScript

	/// <summary>
	/// Simple Datacontainer (inner class) for a Pair of Spell and cooldown
	/// </summary>
    [System.Serializable]
	public struct SpellSlot {
		public A_Spell spell;
		public float cooldown;

        /// <summary>
        /// casts the spell inside the slot and also adjusts the cooldown accordingly
        /// This automatically assumes, that the overlaying PlayerScript's update routine decreases the spellslot's cooldown continuously
        /// </summary>
        /// <SpellslotLambda name="caster"></SpellslotLambda>
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