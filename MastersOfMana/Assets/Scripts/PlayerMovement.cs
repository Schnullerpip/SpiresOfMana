using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public class PlayerMovement : ServerMoveable
{
	[Header("Movement")]
	public float speed = 6;  
	[Range(0,1)]
	public float focusSpeedSlowdown = .25f;
	public float jumpStrength = 5;
	public float additionalFallGravityMultiplier = 1f;
	[Tooltip("How much slower is the player when he/she walks backwards? 0 = no slowdown, 1 = fullstop")]
	[Range(0.0f,1.0f)]
	public float amountOfReverseSlowdown = 0.25f;
	[Tooltip("At which angle does the player still move with fullspeed?")]
	[Range(0.0f,180.0f)]
	public int maxFullspeedAngle = 90;
	[Tooltip("How many meters per second falling is considered too much?")]
	public float fallingDamageThreshold = 18.0f;

	public FeetCollider feet;

	private bool mIsFalling = false;
	private Vector3 mMoveInput;
	private bool mFocusActive;

    public override void Awake()
    {
        base.Awake();
        feet.onLanding += Landing;
    }

    public void OnDisable()
    {
        feet.onLanding -= Landing;
    }

    public void SetMoveInput(Vector3 input)
	{
		mMoveInput = input;
	}

	void FixedUpdate()
	{
		Vector3 direction = mMoveInput * Time.deltaTime * speed * (mFocusActive ? focusSpeedSlowdown : 1);
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

//		Vector3 localDirection = transform.InverseTransformVector(direction);
//		animator.SetFloat("speed_forward",localDirection.x);
//		animator.SetFloat("speed_right",localDirection.z);
//
//		animator.SetFloat("velocity",directionSqrMag);

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

	public delegate void OnLandingWhileFalling(float impactVelocity);
	/// <summary>
	/// This Delegate is called once, when the FeetCollider is touching ground again. Regardless of wether or not the ground is considered steady
	/// </summary>
	public OnLandingWhileFalling onLandingWhileFalling;

	public void Landing()
	{
		if(mIsFalling)
		{
			float delta = - mRigidbody.velocity.y - fallingDamageThreshold;
//			float damage = delta * 3;
			if(onLandingWhileFalling != null)
			{
                onLandingWhileFalling(delta);
			}
		}
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
//			animator.SetTrigger("jump");
		}
	}
}