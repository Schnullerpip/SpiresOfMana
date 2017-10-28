using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FeetCollider : MonoBehaviour 
{
	[Tooltip("Maximum degree which should be considered 'firmly grounded'")]
	public float maxSlope = 60;
	public float currentSlopeAngle;
	public float tolerance = 1.6f;

	/// <summary>
	/// Did the feet have contact with the ground this frame?
	/// </summary>
	private bool mContactThisFrame = false;
	private bool mContactLastFrame = false;
	private bool mIsGrounded = false;
	private Vector3 mGroundNormal;
	private SphereCollider sphereCollider;

	public delegate void OnLanding();
	/// <summary>
	/// This Delegate is called once, when the FeetCollider is touching ground again. Regardless of wether or not the ground is considered steady
	/// </summary>
	public OnLanding onLanding;

	void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
	}
		
	public void OnCollisionStay(Collision collisionInfo)
	{
		foreach (ContactPoint contact in collisionInfo.contacts) 
		{
			if(contact.thisCollider == sphereCollider)
			{
				Collision(contact);
			}
		}
	}

	/// <summary>
	/// Calculates wether or not the Collision on the specified contact should be considered stirdy ground. 
	/// This can happen multiple times per physics step. The normal with the lowest angle is considered ground.
	/// </summary>
	/// <param name="contact">Contact.</param>
	public void Collision(ContactPoint contact)
	{
		mContactThisFrame = true;

		float tempSlope = Vector3.Angle(Vector3.up, contact.normal);

		if(tempSlope < currentSlopeAngle)
		{
			currentSlopeAngle = tempSlope;
			mGroundNormal = contact.normal;
		}
	}

	void FixedUpdate()
	{
		//make some extra checks (only if the terrain is not flat enough for the spherecollider)
		mContactThisFrame = mContactThisFrame || CheckWithTolerance();

		if(mContactThisFrame)
		{
			//this is to prevent landing on a slope (which is not "ground") to take damage multiple times
			if(!mContactLastFrame)
			{
				if(onLanding != null)
				{
					//invoke landing delegate
					onLanding();
				}
			}

			//check ground slopefactor
			mIsGrounded = currentSlopeAngle < maxSlope;
		}
		else
		{
			mIsGrounded = false;
		}
			
		//reset the values for the next collision checks
		currentSlopeAngle = float.MaxValue;

		mContactLastFrame = mContactThisFrame;

		mContactThisFrame = false;
	}

	/// <summary>
	/// Checks for a collision along the last normal with some tolerance.
	/// </summary>
	/// <returns><c>true</c>, if there was a hit with tolerance, <c>false</c> otherwise.</returns>
	private bool CheckWithTolerance()
	{
		//transform from local to worldspace
		Vector3 worldSpacePosition = transform.position;

		RaycastHit hit;
		bool hitGround = Physics.Raycast (worldSpacePosition, -mGroundNormal, out hit, sphereCollider.radius * tolerance);

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
}
