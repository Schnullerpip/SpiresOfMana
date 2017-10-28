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
public class PlayerScript : NetworkBehaviour, IServerMoveable
{
    //member
    public InputStateSystem inputStateSystem;
    public EffectStateSystem effectStateSystem;
    public CastStateSystem castStateSystem;
    public Rigidbody mRigidbody;

    //spellslots
    public SpellSlot[] spellslot = new SpellSlot[3];

    //references the currently chosen spell, among the three available spellslots

    [SyncVar]
    private int mCurrentSpell;
    public SpellSlot GetCurrentspell()
    {
        return spellslot[mCurrentSpell];
    }

    public int GetCurrentspellslotID()
    {
        return mCurrentSpell;
    }

    public void SetCurrentSpellslotID(int idx)
    {
        mCurrentSpell = idx;
        if (mCurrentSpell > 2 || mCurrentSpell < 0)
        {
            mCurrentSpell = 0;
        }
    }

    /// <summary>
    /// holds references to all the coroutines a spell is running, so they can bes stopped/interrupted w4hen a player is for example hit
    /// and can therefore not continue to cast the spell
    /// </summary>
    private List<Coroutine> mSpellRoutines = new List<Coroutine>();

    public void EnlistSpellRoutine(Coroutine spellRoutine)
    {
        mSpellRoutines.Add(spellRoutine);
    }

    public void FlushSpellroutines()
    {
        for (int i = 0; i < mSpellRoutines.Count; ++i)
        {
            StopCoroutine(mSpellRoutines[i]);
        }
        mSpellRoutines = new List<Coroutine>();
    }

	[Header("Movement")][SyncVar]
	public float speed = 4;  
	[Range(0,1)]
	public float focusSpeedSlowdown = .25f;
    [SyncVar]
	public float jumpStrength = 5;
	public float additionalFallGravityMultiplier = 1f;
	[Tooltip("How much slower is the player when he/she walks backwards? 0 = no slowdown, 1 = fullstop")]
	[Range(0.0f,1.0f)]
	public float amountOfReverseSlowdown = 0.0f;
	[Tooltip("At which angle does the player still move with fullspeed?")]
	[Range(0.0f,180.0f)]
	public int maxFullspeedAngle = 90;
	public float fallingDamageThreshold = 18.0f;

	private bool mIsFalling = false;

	[HideInInspector]
	public Vector3 moveInputForce;
	public FeetCollider feet;

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
	public PlayerCamera cameraRigPrefab;
    public PlayerCamera cameraRig;
    public Transform handTransform;
	private Vector3 mAimRefinement;

	public bool mFocusActive = false;
	private Collider mFocusedTarget = null;
	protected Rewired.Player rewiredPlayer;
    public Rewired.Player GetRewired()
    {
        return rewiredPlayer;
    }

    [SyncVar] public string playerName;

    public PlayerHealthScript healthScript;

	[Header("Animation")]
	public Animator animator;
	public Transform headJoint;

	void Awake()
	{
        lookDirection = transform.forward;
		feet.onLanding += Landing;
    }

    private void OnDisable()
    {
        // Fix issue with LateUpdate on camera referencing the player
		if(cameraRig != null)
		{
			cameraRig.gameObject.SetActive(false);
		}
    }

    // Use this for initialization
    public void Start()
    {
        //initialize the statesystems
        inputStateSystem = new InputStateSystem(this);
        effectStateSystem = new EffectStateSystem(this);
        castStateSystem = new CastStateSystem(this);

        //initialize Inpur handler
	    rewiredPlayer = ReInput.players.GetPlayer(0);

        healthScript = GetComponent<PlayerHealthScript>();
        mRigidbody = GetComponent<Rigidbody>();

        //set the currently chosen spell to a default
	    mCurrentSpell = 0;
	}

    [Command]
    private void CmdGiveGo()
    {
        GameManager.instance.Go();
    }

    // Use this for initialization on local Player only
    override public void OnStartLocalPlayer()
    {
        GameManager.instance.localPlayer = this;
        CmdGiveGo();
        cameraRig = Instantiate(cameraRigPrefab);
        cameraRig.GetComponent<PlayerCamera>().followTarget = this;
        if (cameraRig == null)
        {
            Debug.LogWarning("No camera rig assigned, consider creating one during runtime? Or don't. I'm not your boss. kthx");
        }
        cameraRig.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcSetInputState(InputStateSystem.InputStateID id)
    {
        inputStateSystem.SetState(id);
    }
    [ClientRpc]
    public void RpcSetCastState(CastStateSystem.CastStateID id)
    {
        castStateSystem.SetState(id);
    }
    [ClientRpc]
    public void RpcSetEffectState(EffectStateSystem.EffectStateID id)
    {
        effectStateSystem.SetState(id);
    }

    /// method to move the client, even though client has authority over his position
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    public void RpcAddForce(Vector3 force, int mode)
    {
        if (isLocalPlayer)
        {
            mRigidbody.AddForce(force, (ForceMode)mode);
        }
    }

    /// <summary>
    /// adds explosion force to player on server side - kinda
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    [ClientRpc]
    public void RpcAddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        if (isLocalPlayer)
        {
            mRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        }
    }

    [ClientRpc]
    public void RpcAddForceAndUpdatePosition(Vector3 force, ForceMode mode, Vector3 newPosition)
    {
        mRigidbody.AddForce(force, mode);
        mRigidbody.position = newPosition;
    }

    [ClientRpc]
    public void RpcStopMotion()
    {
        mRigidbody.velocity = Vector3.zero;
    }

    //choosing a spell
    [Command]
    public void CmdChooseSpellslot(int idx)
    {
        mCurrentSpell = idx;
        RpcSetCastState(CastStateSystem.CastStateID.Normal);
    }

    //casting the chosen spell
    [Command]
    public void CmdCastSpell()
    {
        spellslot[mCurrentSpell].Cast(this);
    }

    /// <summary>
    /// the direction the player aims during a cast (this field only is valid, during a cast routine, on the server!
    /// </summary>
    private Vector3 mAimDirection;
    public Vector3 GetAimDirection()
    {
        return mAimDirection;
    }
    private Vector3 mCameraPosition;
    public Vector3 GetCameraPosition()
    {
        return mCameraPosition;
    }
    private Vector3 mCameraLookdirection;
    public Vector3 GetCameraLookDirection()
    {
        return mCameraLookdirection;
    }


    //resolving the chosen spell
    [Command]
    public void CmdResolveSpell(Vector3 aimDirection, Vector3 CameraPostion, Vector3 CameraLookDirection)
    {
        mAimDirection = aimDirection;
        mCameraPosition = CameraPostion;
        mCameraLookdirection = CameraLookDirection;

        spellslot[mCurrentSpell].Cast(this);
    }

    // Update is called once per frame
    void Update () 
	{
	    // Update only on the local player
	    if (!isLocalPlayer)
	    {
            return;
	    }

        //update the states
        inputStateSystem.Update();
        castStateSystem.Update();
        effectStateSystem.Update();

        if(!healthScript.IsAlive())
            animator.SetBool("isDead", true);

        lookDirection = Quaternion.AngleAxis(-yAim, transform.right) * transform.forward;
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
		//propagate the collsionenter event to the feet
		feet.OnCollisionStay(collisionInfo);
	}
		
	void FixedUpdate()
	{
		animator.SetBool("grounded", feet.IsGrounded());

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
			mRigidbody.velocity += Physics.gravity * additionalFallGravityMultiplier * Time.deltaTime;
		}

		float directionSqrMag = direction.sqrMagnitude;

		//move the character
		mRigidbody.MovePosition(mRigidbody.position + direction);

		Vector3 localDirection = transform.InverseTransformVector(direction);
		animator.SetFloat("speed_forward",localDirection.x);
		animator.SetFloat("speed_right",localDirection.z);

		animator.SetFloat("velocity",directionSqrMag);

		//if the player gets input
		if(directionSqrMag > 0)
		{
			//calculate the angle between the movemement and external forces
			float angle = Vector2.Angle(mRigidbody.velocity.xz(),directionXZ);
			//move the rigidbody's velocity towards zero in the xz plane, proportional to the angle
			mRigidbody.velocity = Vector3.MoveTowards(mRigidbody.velocity, new Vector3(0,mRigidbody.velocity.y,0), speed * Time.deltaTime * angle / 180);
		}

		mIsFalling = mRigidbody.velocity.y <= -fallingDamageThreshold;
	}
		
	public void Landing()
	{
		if(mIsFalling)
		{
			float delta = - mRigidbody.velocity.y - fallingDamageThreshold;
			float damage = delta * 3;
			print("Falling Damage: "+damage);
            healthScript.TakeFallDamage(damage);
		}
	}

	void LateUpdate()
	{
		//rotate the head joint, do this in the lateupdate to override the animation (?)
		headJoint.localRotation = Quaternion.AngleAxis(-yAim,Vector3.right); 
	}


	/// <summary>
	/// Let's the character jump with the default jumpStength
	/// </summary>
	public void Jump(bool onlyIfGrounded = true)
	{
		Jump(jumpStrength, onlyIfGrounded);
	}

	/// <summary>
	/// Let's the character jump with a specified jumpStrength
	/// </summary>
	/// <SpellslotLambda name="jumpForce">Jump force.</SpellslotLambda>
	public void Jump(float jumpStrength, bool onlyIfGrounded)
	{
		if(feet.IsGrounded() || !onlyIfGrounded)
		{
			mRigidbody.AddForce(Vector3.up * jumpStrength,ForceMode.VelocityChange);
			animator.SetTrigger("jump");
		}
	}

    ////Remote Procedure Calls!
    //[ClientRpc]
    //public void RpcChangeInputState(InputStateSystem.InputStateID newStateID)
    //{
    //    inputStateSystem.SetState(newStateID);
    //}

    /// <summary>
    /// This method actually updates the spells
    /// </summary>
    /// <param name="spell1"></param>
    /// <param name="spell2"></param>
    /// <param name="spell3"></param>
    public void UpdateSpells(int spell1, int spell2, int spell3)
    {
        Prototype.NetworkLobby.LobbyManager NetworkManager = Prototype.NetworkLobby.LobbyManager.s_Singleton;
        if (NetworkManager)
        { 
            SpellRegistry spellregistry = NetworkManager.mainMenu.spellSelectionPanel.GetComponent<SpellSelectionPanel>().spellregistry;
            if (spellregistry)
            {
                spellslot[0].spell = spellregistry.GetSpellByID(spell1);
                spellslot[1].spell = spellregistry.GetSpellByID(spell2);
                spellslot[2].spell = spellregistry.GetSpellByID(spell3);
            }
        }
    }

    /// <summary>
    /// Update spells on client side
    /// </summary>
    /// <param name="spell1"></param>
    /// <param name="spell2"></param>
    /// <param name="spell3"></param>
    [ClientRpc]
    public void RpcUpdateSpells(int spell1, int spell2, int spell3)
    {
        UpdateSpells(spell1, spell2, spell3);
    }

    /// <summary>
    /// allows the server and thus the spells, to affect the players position
    /// </summary>
    /// <param name="vec3"></param>
    [ClientRpc]
    public void RpcSetPosition(Vector3 vec3)
    {
        this.transform.position = vec3;
    }

    //useful asstes for the PlayerScript

    /// <summary>
    /// Simple Datacontainer (inner class) for a Pair of Spell and cooldown
    /// </summary>
    [System.Serializable]
	public class SpellSlot {
		public A_Spell spell;
		public float cooldown;

        /// <summary>
        /// activates the casting animation, after the spells castduration it activates the 'holding spell' animation
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="castDuration"></param>
        /// <returns></returns>
        private IEnumerator CastRoutine(PlayerScript caster, float castDuration)
        {
            //set caster in 'casting mode'
            caster.RpcSetCastState(CastStateSystem.CastStateID.Resolving);

            yield return new WaitForSeconds(castDuration);
            //resolve the spell
            spell.Resolve(caster);

            //set caster in 'normal mode'
            caster.RpcSetCastState(CastStateSystem.CastStateID.Normal);
        }

        /// <summary>
        /// casts the spell inside the slot and also adjusts the cooldown accordingly
        /// This automatically assumes, that the overlaying PlayerScript's update routine decreases the spellslot's cooldown continuously
        /// This should only be called on the server!!
        /// </summary>
	    public void Cast(PlayerScript caster)
	    {
            if (cooldown <= 0)
            {
                //start the switch to 'holding spell' animation after the castduration
                caster.EnlistSpellRoutine(caster.StartCoroutine(CastRoutine(caster, spell.castDurationInSeconds)));
            }
        }
	}

	//get default parameters
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }
}