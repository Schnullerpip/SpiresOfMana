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
	[Tooltip("Degrees per seconds")]
	public float aimSpeed = 360;    
	public float jumpStrength = 5;

	private Rigidbody rigid;

	public Vector3 moveInputForce;
	public float yAim = 0;

	public Vector3 lookDirection;

	protected Rewired.Player mRewiredPlayer;

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

		//store the aim input, either mouse or right analog stick
		Vector2 aimInput = mRewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical");
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
		if(mRewiredPlayer.GetButtonDown("Jump"))
		{
			mInputStateSystem.current.Jump();
		}

		//TODO: define mouse & keyboard / controller schemes, "CastSpell" not final axis name
		if(mRewiredPlayer.GetButtonDown("CastSpell"))
		{
			mInputStateSystem.current.Cast_Spell_1();
		}

        mInputStateSystem.current.Move(movementInput);
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
	}

	/// <summary>
	/// Let's the character jump with the default jumpStength
	/// </summary>
	public void Jump()
	{
		Jump(jumpStrength);
	}

	/// <summary>
	/// Let's the character jump with a specified jumpStength
	/// </summary>
	/// <param name="jumpForce">Jump force.</param>
	public void Jump(float jumpStrength)
	{
		//TODO grounded
		rigid.AddForce(Vector3.up * jumpStrength,ForceMode.VelocityChange);
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