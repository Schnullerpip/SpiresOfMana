using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public class PlayerAim : NetworkBehaviour {

	[Tooltip("Degrees per seconds")]
	public float aimSpeed = 360;    

	[Header("Focus Assist")]
//	public float aimAssistInUnits = 1.0f;
	[Tooltip("How fast is the aim when focused? 0 = freeze, 1 = no slowdown")]
	[Range(0,1)]
	public float focusAimSpeedFactor = .25f;

	public float focusAssistSpeed = 20;

	public float maxYAngle = 85;

//	[Tooltip("How many units can the player move the cursor when locked on?")]
//	public float maxAimRefinementMagnitude = 1f;
//	[HideInInspector]
	private float yAngle = 0;
//	[HideInInspector][SyncVar]
//	public Vector3 lookDirection;
	public Transform handJoint;

	public Quaternion currentLookRotation;

//	private Vector3 mAimRefinement;
	private PlayerCamera mCameraRig;
	private bool mFocusActive = false;
	private Collider mFocusedTarget = null;

	void Awake()
	{
//		lookDirection = transform.forward;
	}

	void OnDisable()
	{
		// Fix issue with LateUpdate on camera referencing the player
		if(mCameraRig != null)
		{
			mCameraRig.gameObject.SetActive(false);
		}
	}

	public PlayerCamera GetCameraRig(){
		return mCameraRig;
	}

	public void SetCameraRig(PlayerCamera cameraRig){
		mCameraRig = cameraRig;
	}

	public float GetYAngle(){
		return yAngle;
	}

	void Update()
	{
		if(isLocalPlayer)
		{
//			lookDirection = Quaternion.AngleAxis(-yAngle, transform.right) * transform.forward;

			//calculate the lookRotation by taking the yAngle (up and down) and the rotation in the y Axis (in form of the vec3 forward)
			currentLookRotation = Quaternion.LookRotation(Quaternion.AngleAxis(yAngle, transform.right) * transform.forward);
		}
	}

	void LateUpdate()
	{
		if(isLocalPlayer)
		{
			Vector3 rot = handJoint.localRotation.eulerAngles;
			rot.x = yAngle;
			handJoint.localRotation = Quaternion.Euler(rot);
		}
	}

	/// <summary>
	/// Determines whether this instance has a focus target.
	/// </summary>
	/// <returns><c>true</c> if this instance has focus target; otherwise, <c>false</c>.</returns>
	public bool HasFocusTarget()
	{
		return mFocusedTarget != null;
	}

//	/// <summary>
//	/// Validates if the focus target is in view. Sets the FocusTarget to null if not in free view.
//	/// </summary>
//	/// <returns><c>true</c>, if focus target view was validated, <c>false</c> otherwise.</returns>
//	public bool ValidateFocusTargetView ()
//	{
//		if(mFocusedTarget == null)
//		{
//			return false;
//		}
//		RaycastHit hit;
//		//check if the focus target is still in view
//		cameraRig.RaycastCheck (mFocusedTarget.bounds.center, out hit);
//		if (hit.collider.gameObject != mFocusedTarget.gameObject) 
//		{
//			mFocusedTarget = null;
//			return false;
//		}
//		return true;
//	}

	/// <summary>
	/// Aims the player by the specified aimMovement.
	/// </summary>
	/// <param name="aimMovement">Aim movement.</param>
	public void Aim (Vector2 aimMovement)
	{
		aimMovement *= Time.deltaTime * aimSpeed * (mFocusActive ? focusAimSpeedFactor : 1);

		//rotate the entire player along its y-axis
		transform.Rotate (0, aimMovement.x, 0);
		//prevent spinning around the z-Axis (no backflips allowed)
		yAngle = Mathf.Clamp (yAngle - aimMovement.y, -maxYAngle, maxYAngle);
	}

//	/// <summary>
//	/// Refines the aim.
//	/// </summary>
//	/// <param name="aimInput">Aim input.</param>
//	public void RefineAim (Vector2 aimInput, Rewired.ControllerType controllerType)
//	{
//		aimInput *= Time.deltaTime * aimSpeed * (mFocusActive ? focusAimSpeedFactor : 1);
//
//		//convert from local to worldspace and reduces the input by pi/2 since it was parameterized for angular movement of player
//		mAimRefinement += transform.TransformDirection (aimInput) / Mathf.PI * .5f;
//		mAimRefinement = Vector3.ClampMagnitude (mAimRefinement, maxAimRefinementMagnitude);
//
//		//do the following only if the player uses a joystick, as this rubberbanding is just super buggy with a mouse
//		if(controllerType == Rewired.ControllerType.Joystick)
//		{
//			if (aimInput.sqrMagnitude <= 0) 
//			{
//				mAimRefinement = Vector3.MoveTowards (mAimRefinement, Vector3.zero, Time.deltaTime * aimSpeed * Time.deltaTime);
//			}
//		}
//	}
//
//	public void ResetRefinement()
//	{
//		mAimRefinement = Vector3.zero;
//	}

	public void LookAtFocusTarget(){
		LookAt(mFocusedTarget.bounds.center);
	}

	/// <summary>
	/// Rotates the player towards the focus target.
	/// </summary>
	public void LookAt (Vector3 pointInWorldSpace)
	{
		//get the direction to the target, from the actual cameras position
		Vector3 dirToTarget = (pointInWorldSpace /*+ mAimRefinement*/) - mCameraRig.GetCamera().transform.position;

		//rotate towards the the direction of the target over multiple frames
		//at short distances this is usually 1 or 2 frames, but when doing bigger rotations this should help with the view snapping effect
		Quaternion targetRotation = Quaternion.RotateTowards (currentLookRotation, Quaternion.LookRotation (dirToTarget), focusAssistSpeed * Time.deltaTime);

		Vector3 rotationEuler = targetRotation.eulerAngles;

		//take the calculated quaternuon and split it into a rotation along the players y axis
		transform.rotation = Quaternion.Euler(0, rotationEuler.y, 0);
		//and the yAngle (which is the angle along the players x-Axis, maybe this name should be refactored?
		yAngle = rotationEuler.x;

		//since quaterions go from 0 - 360 and then wrap around, this is a workaround to get the desired value between -maxYAngle and maxYAngle
		if(yAngle > maxYAngle)
		{
			yAngle -= 360;
		}

		yAngle = Mathf.Clamp(yAngle, -maxYAngle, maxYAngle);

//		yAngle = Vector3.SignedAngle (transform.forward, dirToTarget, -transform.right);
//		Quaternion lookRotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
//
//		Vector3 lookRotationEuler = lookRotation.eulerAngles;
//
//		transform.rotation = Quaternion.Euler(0,lookRotation.y * Mathf.Rad2Deg,0);

		/*
		float yRotation = Mathf.Atan2 (dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;

		Vector3 rot = transform.rotation.eulerAngles;
		rot.y = Mathf.MoveTowardsAngle(rot.y, yRotation, 5);
		transform.rotation = Quaternion.Euler(rot);

//		transform.rotation = Quaternion.AngleAxis (Mathf.MoveTowards(transform.rotation.y, yRotation, 5), Vector3.up);
		yAngle = Mathf.MoveTowardsAngle(yAngle, -Vector3.SignedAngle (transform.forward, dirToTarget, transform.right), 5);
		*/
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
	public void StartFocus(PlayerScript self)
	{
		mFocusActive = true;
		mFocusedTarget = FocusAssistTarget(self);//, aimAssistInUnits);
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
	private Collider FocusAssistTarget(PlayerScript self)//, float maxUnitsOff)
	{
		//shoot a raycast in the middle of the screen
		RaycastHit hit;
		self.SetColliderIgnoreRaycast(true);

		if(mCameraRig.CenterRaycast(out hit))
		{
			self.SetColliderIgnoreRaycast(false);
			//if we hit a healthscript, take that as our aim assist target
			PlayerScript target = hit.collider.GetComponentInParent<PlayerScript>();
			if(target != null)
			{
				self.SetColliderIgnoreRaycast(false);
				return hit.collider;
			}
		}
		//otherwise look for the next best target

		//TODO: cache this, maybe gamemanager?
		List<PlayerScript> allPlayers = new List<PlayerScript>(GameObject.FindObjectsOfType<PlayerScript>());

//		//sort by distance 
//		allHealthScripts.Sort(
//			delegate(HealthScript a, HealthScript b) 
//			{
//				return Vector3.Distance(transform.position,a.transform.position).CompareTo(Vector3.Distance(transform.position,b.transform.position));
//			}
//		);

		//sort by angle
		allPlayers.Sort(
			delegate(PlayerScript a, PlayerScript b) 
			{
				var camPos = mCameraRig.GetCamera ().transform.position;
				var eulerAngles = currentLookRotation.eulerAngles;
				return Vector3.Angle(eulerAngles, a.transform.position - camPos)
					.CompareTo(Vector3.Angle (eulerAngles, b.transform.position - camPos));
			}
		);

		//iterate through all healthscripts
		foreach (PlayerScript aPlayer in allPlayers) 
		{
			//skip player him/herself
			if(aPlayer.gameObject == this.gameObject)
			{
				continue;
			}
				
//			//skip if the target is behind the player
//			if(transform.InverseTransformPoint(aHealthScript.transform.position).z < 0)
//			{
//				continue;
//			}

			Collider healthScriptCollider = aPlayer.GetComponentInChildren<Collider>();

			//get the vector to the target, from the position of the camera
//			Vector3 dirToTarget = healthScriptCollider.bounds.center - cameraRig.GetCamera().transform.position;

			//priject the direction vector to the target onto a plane, defined by the lookDirection
			//this way it acts as if the target hat a kind of 2d hitbox (circle) that expands maxUnitsOff into every direciton 
//			if(Vector3.ProjectOnPlane(dirToTarget, - lookDirection).sqrMagnitude < maxUnitsOff * maxUnitsOff)
			{
//				if(cameraRig.RaycastCheck(healthScriptCollider.bounds.center, out hit))
//				Debug.DrawLine(mCameraRig.GetCamera().transform.position, healthScriptCollider.bounds.center, Color.red, 20);
				if(Physics.Linecast(mCameraRig.GetCamera().transform.position, healthScriptCollider.bounds.center, out hit))
				{
					//TODO find a better method to varify target, perhabs tags?
					if(hit.collider.GetComponentInParent<PlayerScript>() == aPlayer)
					{
						self.SetColliderIgnoreRaycast(false);
						return hit.collider;
					}
				}
			}
		}

		//nothing found
		self.SetColliderIgnoreRaycast(false);
		return null;
	}
}
