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
    /// holds references to all the coroutines a spell is running, so they can bes stopped/interrupted when a player is for example hit
    /// and can therefore not continue to cast the spell
    /// </summary>
    public List<IEnumerator> spellRoutines = new List<IEnumerator>();

    public void EnlistCoroutine(IEnumerator spellRoutine)
    {
        spellRoutines.Add(spellRoutine);
    }

	[Header("Movement")]
	public float speed = 4;  
	public float jumpStrength = 5;
	public float fallGravityMultiplier = 1.2f;
	[Range(0.0f,1.0f)]
	[Tooltip("How much slower is the player when he/she walks backwards? 0 = no slowdown, 1 = fullstop")]
	public float amountOfReverseSlowdown = 0.0f;
	[Range(0.0f,180.0f)]
	[Tooltip("At which angle does the player still move with fullspeed?")]
	public int maxFullspeedAngle = 90;

	[HideInInspector]
	public Vector3 moveInputForce;
	public FeetGroundCheck feet;

	[Header("Aim")]
	[Tooltip("Degrees per seconds")]
	public float aimSpeed = 360;    
	public float aimAssist = 10;
	[HideInInspector]
	public float yAim = 0;
	[HideInInspector]
	public Vector3 lookDirection;
	public PlayerCamera cameraRig;
	public Transform handTransform;

	private Rigidbody mRigidbody;

	protected Rewired.Player rewiredPlayer;
	private HealthScript mFocusedTarget = null;

	void Awake()
	{
		lookDirection = transform.forward;

		if(cameraRig == null)
		{
			Debug.LogWarning("No camera rig assigned, consider creating one during runtime? Or don't. I'm not your boss. kthx");
		}
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
		#endregion


		#region Aiming
		//store the aim input, either mouse or right analog stick
		Vector2 aimInput = rewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical");
		aimInput = Vector3.ClampMagnitude(aimInput,1);

		//take framerate into consideration
		aimInput *= Time.deltaTime * aimSpeed;

		if(mFocusedTarget != null)
		{
			//get the direction to the target, from the actual cameras position
			Vector3 dirToTarget = mFocusedTarget.transform.position - cameraRig.GetCamera().transform.position;
			//rotate towards the target
			float yRotation = Mathf.Atan2 (dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (yRotation, Vector3.up);
			yAim = -Vector3.SignedAngle(transform.forward,dirToTarget,transform.right);
		}
		else
		{
			//rotate the entire player along its y-axis
			transform.Rotate(0,aimInput.x,0);
			//prevent spinning around the z-Axis (no backflips allowed)
			yAim = Mathf.Clamp(yAim + aimInput.y, -89, 89);
		}

		//calculate the lookDirection vector with the current forward vector as a basis, rotating up or down
		lookDirection = Quaternion.AngleAxis(-yAim, transform.right) * transform.forward;

		#endregion

		#if UNITY_EDITOR 
		Debug.DrawRay(transform.position+Vector3.up*1.8f, lookDirection, Color.red);
		#endif
 	}

	public void StartFocus()
	{
		mFocusedTarget = FocusAssistTarget(aimAssist);
	}

	public void StopFocus()
	{
		mFocusedTarget = null;
	}

	/// <summary>
	/// Returns a HealthScript thats within the specified 
	/// maxAngle of the camera and that is not obstructed.
	/// </summary>
	/// <returns>The assist target.</returns>
	/// <param name="maxAngle">Max angle.</param>
	private HealthScript FocusAssistTarget(float maxAngle)
	{
		HealthScript[] allHealthScripts = GameObject.FindObjectsOfType<HealthScript>();

		foreach (HealthScript potentialTarget in allHealthScripts) 
		{
			if(potentialTarget.gameObject == this)
				continue;

			Vector3 dirToTarget = potentialTarget.transform.position - cameraRig.GetCamera().transform.position;
			if(Vector3.Angle(dirToTarget,lookDirection) < maxAngle)
			{
				RaycastHit hit;
				if(Physics.Raycast(cameraRig.GetCamera().transform.position,dirToTarget,out hit))
				{
					if(hit.collider.gameObject == potentialTarget.gameObject)
					{
						return potentialTarget;
					}
				}
			}
		}

		return null;
	}

	//TODO: delete this method, testing purpose only
	/// <summary>
	/// !TESTING PURPOSE ONLY! 
	/// This methods creates a GameObject with a LineRenderer to display the line between Hand and Raycast Hit by mouse.
	/// </summary>
	/// <param name="worldSpacePosition">World space position.</param>
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

		Vector3 direction = moveInputForce * Time.deltaTime * speed;
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
	/// <param name="jumpForce">Jump force.</param>
	public void Jump(float jumpStrength)
	{
		if(feet.IsGrounded())
		{
			mRigidbody.AddForce(Vector3.up * jumpStrength,ForceMode.VelocityChange);
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