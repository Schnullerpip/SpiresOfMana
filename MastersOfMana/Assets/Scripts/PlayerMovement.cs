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

	public float hurtSlowdown = 0.5f;

	[Range(0,1)]
	public float inputVelocityInfluence = 0.75f;

	public FeetCollider feet;

	private bool mIsFalling = false;
	private Vector3 mMoveInput;
	private bool mFocusActive;

	private bool mHurtSlowdownActive;

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

	public void SetMoveInputHurt(Vector3 input)
	{
		mHurtSlowdownActive = true;
		mMoveInput = input;
	}

	public delegate void OnMovement(float velocityMag, Vector2 direction, bool isGrounded);
	public OnMovement onMovement;

	void FixedUpdate()
	{
		Vector3 movement = mMoveInput * speed;

		movement *= mFocusActive ? focusSpeedSlowdown : 1;
		movement *= mHurtSlowdownActive ? hurtSlowdown : 1;

		Vector2 movementXZ = movement.xz();

		//calculate the amount of slowdown, by comparing the direction with the forward vector of the character
		Vector2 forwardXZ = transform.forward.xz();
		//value between 0 and 1, 1 being total reversaldamping, 0 being no damping
		float reverseDamping = Mathf.Clamp01((Vector2.Angle (forwardXZ, movementXZ) - maxFullspeedAngle) / 180 * 2);
		reverseDamping *= amountOfReverseSlowdown;
		movement *= 1 - reverseDamping;

		//increase the falling speed to make it feel a bit less floaty
		if(mRigidbody.velocity.y < 0)
		{
			mRigidbody.velocity += Physics.gravity * additionalFallGravityMultiplier * Time.deltaTime;
		}

		Vector3 newMovement = movement;

		//if the player gets input
		if(movement.sqrMagnitude > 0)
		{
			//calculate the angle between the movemement and external forces
			float angle = Vector2.Angle(mRigidbody.velocity.xz(),movementXZ);
			//the anglefactor is a value between 0 (same direction) and 1 (opposite direction)
			float angleFactor = angle / 180;

			float influence = speed * Time.deltaTime * angleFactor;
			Vector3 desiredVelocity = new Vector3(0,mRigidbody.velocity.y,0);
			//move the rigidbody's velocity towards zero in the xz plane, proportional to the angle
			mRigidbody.velocity = Vector3.MoveTowards(mRigidbody.velocity, desiredVelocity, influence * inputVelocityInfluence);

			//the following steps are necessary to allow the player to move while having external forces applied to him/her

			//early exit if the velocity is to small to be relevant for the calculations
			if(mRigidbody.velocity.sqrMagnitude > 0.0001f)
			{
				//cache velcity as a 2D vector in the XZ plane, Y is still only handled by the physicsengine 
				Vector2 veloXZ = mRigidbody.velocity.xz();
				//also cache magnitude, as it is a bit more expansive to calculate (squareroot)
				float veloXZMagnitude = veloXZ.magnitude;

				Vector2 accumulatedVectors = veloXZ + movementXZ;
				accumulatedVectors = Vector2.ClampMagnitude(accumulatedVectors, Mathf.Max(veloXZMagnitude, speed));
				Vector2 newMovementXZ = accumulatedVectors - veloXZ;

				//if the velocity was higher than the maximum speed, reduce the new direction by a factor corresponding to the angle 
				if(veloXZMagnitude > speed)
				{
					newMovementXZ *= (1-angleFactor);
				}

				//stuff it into the vector3
				newMovement.x = newMovementXZ.x;
				newMovement.z = newMovementXZ.y;
			}

			mRigidbody.MovePosition(mRigidbody.position + newMovement * Time.deltaTime);
		}

		Vector3 localMovement = transform.InverseTransformVector(newMovement);

		if(onMovement != null)
		{
			onMovement(newMovement.magnitude, localMovement.xz(), feet.IsGrounded());
		}

		mIsFalling = mRigidbody.velocity.y <= -fallingDamageThreshold;

		mHurtSlowdownActive = false;

	}

	public Vector3 GetVelocity()
	{
		return mRigidbody.velocity;
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

	public delegate void OnJumping();
	public OnJumping onJumping;

	/// <summary>
	/// Let's the character jump with a specified jumpStrength
	/// </summary>
	/// <SpellslotLambda name="jumpForce">Jump force.</SpellslotLambda>
	public void Jump(float jumpStrength, bool onlyIfGrounded)
	{
		if(feet.IsGrounded() || !onlyIfGrounded)
		{
			mRigidbody.AddForce(Vector3.up * jumpStrength,ForceMode.VelocityChange);
			if(onJumping != null)
			{
				onJumping();
			}
		}
	}
}