using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAim : NetworkBehaviour {

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
	public float yAngle = 0;
	[HideInInspector][SyncVar]
	public Vector3 lookDirection;
	public PlayerCamera cameraRig;
	public Transform handTransform;

	private Vector3 mAimRefinement;
	private bool mFocusActive = false;
	private Collider mFocusedTarget = null;

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
		yAngle = Mathf.Clamp (yAngle + aimMovement.y, -89, 89);
	}

	/// <summary>
	/// Refines the aim.
	/// </summary>
	/// <param name="aimInput">Aim input.</param>
	public void RefineAim (Vector2 aimInput, Rewired.ControllerType controllerType)
	{
		//convert from local to worldspace and reduces the input by pi/2 since it was parameterized for angular movement of player
		mAimRefinement += transform.TransformDirection (aimInput) / Mathf.PI * .5f;
		mAimRefinement = Vector3.ClampMagnitude (mAimRefinement, maxAimRefinementMagnitude);

		//do the following only if the player uses a joystick, as this rubberbanding is just super buggy with a mouse
		if(controllerType == Rewired.ControllerType.Joystick)
		{
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
		yAngle = -Vector3.SignedAngle (transform.forward, dirToTarget, transform.right);
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
}
