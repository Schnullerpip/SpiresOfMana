using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public class PlayerMovement : ServerMoveable
{
	[Header("Movement")]
    public float originalSpeed = 4;
	public float speed = 4;  
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

    public float mMaxVelocityMagnitude;

	public float hurtSlowdown = 0.5f;

	[Range(0,1)]
	public float inputVelocityInfluence = 0.75f;

	public FeetCollider feet;

    public SoundEffects soundEffects;

	private bool mIsFalling = false;
	private Vector3 mMoveInput;
	private bool mFocusActive;

    private bool mHurtSlowdownActive;

	private Vector3 mDeltaPos;
	private Vector3 mLastPos;

    public override void Awake()
    {
        base.Awake();
        speed = originalSpeed;
        feet.onLanding += Landing;
		mLastPos = mRigidbody.position;
    }

    public void OnDisable()
    {
        feet.onLanding -= Landing;
    }

    public void SetMoveInput(Vector3 input)
	{
		//clamp it to prevent faster diagonal movement
		mMoveInput = Vector3.ClampMagnitude(input,1);
	}

    public void ClearMovementInput()
    {
        mMoveInput = Vector3.zero;
    }

	public void SetMoveInputHurt(Vector3 input)
	{
		mHurtSlowdownActive = true;
		//clamp it to prevent faster diagonal movement
		mMoveInput = Vector3.ClampMagnitude(input,1);
	}

	public void SetFocusActive(bool value){
		mFocusActive = value;
	}

	/// <summary>
	/// Gets the movement since last frame.
	/// </summary>
	/// <returns>The delta movement.</returns>
	public Vector3 GetDeltaMovement()
	{
		return mDeltaPos;
	}

	public delegate void OnMovement(float velocityMag, Vector2 direction, bool isGrounded);
	public OnMovement onMovement;

	void FixedUpdate()
	{
		Vector3 movement = mMoveInput;

        movement *= speed;

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
            float mag = newMovement.magnitude;
            onMovement(mag, localMovement.xz(), feet.IsGrounded());
		}

		mIsFalling = mRigidbody.velocity.y <= -fallingDamageThreshold;

		mHurtSlowdownActive = false;

		mDeltaPos = mRigidbody.position - mLastPos;
		mLastPos = mRigidbody.position;

        //adjust velocity if its to high
	    var squaredMax = mMaxVelocityMagnitude*mMaxVelocityMagnitude;
	    if (mRigidbody.velocity.sqrMagnitude > squaredMax)
	    {
            mRigidbody.velocity = Vector3.ClampMagnitude(mRigidbody.velocity, mMaxVelocityMagnitude);
	    }
	}

	public delegate void OnLandingWhileFalling(int impactVelocity);
	/// <summary>
	/// This Delegate is called once, when the FeetCollider is touching ground again. Regardless of wether or not the ground is considered steady
	/// </summary>
	public OnLandingWhileFalling onLandingWhileFalling;

	public void Landing()
	{
		if(mIsFalling)
		{
            int delta = Mathf.RoundToInt(-mRigidbody.velocity.y - fallingDamageThreshold);

			if(onLandingWhileFalling != null)
			{
                onLandingWhileFalling(delta);
			}
		}

        soundEffects.PlayLandingSFX();
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
			mRigidbody.SetVelocityY(jumpStrength);
			
			if(onJumping != null)
			{
				onJumping();
			}
		}
	}

    /// <summary>
    /// Gets the anticipated position in n-seconds if the player keeps his current speed and direction.
    /// </summary>
    /// <returns>The anticipation position.</returns>
    /// <param name="seconds">Seconds.</param>
    public Vector3 GetAnticipationPosition(float seconds)
    {
        Vector3 prediction = mRigidbody.position;
        prediction += GetCurrentMovementVector() * seconds;
        return prediction;
    }

    public Vector3 GetCurrentMovementVector()
    {
        return GetDeltaMovement() / Time.fixedDeltaTime;
    }

    /// <summary>
    /// This method is called from the player animator. The event is triggered when a foot is touching the ground.
    /// </summary>
    private void Step()
    {
        soundEffects.PlayStepSound();
    }

    [System.Serializable]
    public class SoundEffects
    {
        public AudioClip[] stepSounds;
        public AudioSource sfxSource;

        public PitchingAudioClip landingSfx;

        public void PlayStepSound()
        {
            sfxSource.pitch = 1;
            sfxSource.PlayOneShot(stepSounds.RandomElement());
        }

        public void PlayLandingSFX()
        {
            sfxSource.pitch = landingSfx.GetRandomPitch();
            sfxSource.PlayOneShot(landingSfx.audioClip);
        }
    }
}