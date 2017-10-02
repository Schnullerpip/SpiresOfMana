using UnityEngine;

[System.Serializable]
public class FeetGroundCheck
{
	public Collider collider;
	[Tooltip("Maximum degree which should be considered 'firmly grounded'")]
	public float maxSlope = 60;
	public float currentSlopeAngle;

	/// <summary>
	/// Did the feet have contact with the ground this frame?
	/// </summary>
	private bool mHadContactThisFrame = false;
	private bool mIsGrounded = false;
	private Vector3 mGroundNormal;

	/// <summary>
	/// Clears certain flags and validates if the ground was really touched since the last PhysicsUpdate(). This method should be called once per FixecUpdate()
	/// </summary>
	public void PhysicsUpdate()
	{
		if(mHadContactThisFrame)
		{
			mIsGrounded = currentSlopeAngle < maxSlope;
		}
		else
		{
			mIsGrounded = false;
		}

		currentSlopeAngle = float.MaxValue;
		mHadContactThisFrame = false;
	}

	public bool IsGrounded()
	{
		return mIsGrounded;
	}

	public Vector3 GetGroundNormal()
	{
		return mGroundNormal;
	}

	/// <summary>
	/// Calculates wether or not the Collision on the specified contact should be considered steady ground.
	/// </summary>
	/// <param name="contact">Contact.</param>
	public void Collision(ContactPoint contact)
	{
		mHadContactThisFrame = true;

		float tempSlope = Vector3.Angle(Vector3.up, contact.normal);

		if(tempSlope < currentSlopeAngle)
		{
			currentSlopeAngle = tempSlope;
			mGroundNormal = contact.normal;
		}
	}
}