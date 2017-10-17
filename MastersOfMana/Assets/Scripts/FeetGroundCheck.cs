using UnityEngine;

[System.Serializable]
public class FeetGroundCheck
{
	public SphereCollider collider;
	[Tooltip("Maximum degree which should be considered 'firmly grounded'")]
	public float maxSlope = 60;
	public float currentSlopeAngle;
	public float tolerance = 1.6f;

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
		if(mHadContactThisFrame || CheckWithTolerance())
		{
			mIsGrounded = currentSlopeAngle < maxSlope;
		}
		else
		{
			mIsGrounded = false;
		}
			
		//reset the values for the next collision checks
		currentSlopeAngle = float.MaxValue;
		mHadContactThisFrame = false;
	}

	/// <summary>
	/// Checks for a collision along the last normal with some tolerance.
	/// </summary>
	/// <returns><c>true</c>, if there was a hit with tolerance, <c>false</c> otherwise.</returns>
	private bool CheckWithTolerance()
	{
		//transform from local to worldspace
		Vector3 worldSpacePosition = collider.transform.TransformPoint (collider.center);

		RaycastHit hit;
		bool hitGround = Physics.Raycast (worldSpacePosition, -mGroundNormal, out hit, collider.radius * tolerance);

		if(hitGround)
		{
			currentSlopeAngle = Vector3.Angle(Vector3.up, hit.normal);
		}

		return hitGround;
	}

	/// <summary>
	/// Determines whether this instance is grounded.
	/// </summary>
	/// <returns><c>true</c> if this instance is grounded; otherwise, <c>false</c>.</returns>
	public bool IsGrounded()
	{
		return mIsGrounded;
	}

	/// <summary>
	/// Gets the ground normal.
	/// </summary>
	/// <returns>The ground normal.</returns>
	public Vector3 GetGroundNormal()
	{
		return mGroundNormal;
	}

	/// <summary>
	/// Calculates wether or not the Collision on the specified contact should be considered stirdy ground. 
	/// This can happen multiple times per physics step. The normal with the lowest angle is considered ground.
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