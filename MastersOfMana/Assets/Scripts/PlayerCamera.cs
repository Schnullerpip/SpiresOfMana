using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This components follows a transform with a certain amount of damping.
/// </summary>
public class PlayerCamera : MonoBehaviour {

	public Transform joint;
	public PlayerScript followTarget;

	[Range(0,1)]
	public float movementDamping = .2f;

	[ReadOnlyAttribute]
	public Vector3 movementVelocity; 

	private Vector3 mOffset;

	private Camera mCamera;

	public Camera GetCamera()
	{
		return mCamera;
	}

	void Awake()
	{
		mCamera = GetComponentInChildren<Camera>();

		if(followTarget == null)
		{
			Debug.LogWarning("No Follow Target assigned.", this.gameObject);
//			enabled = false;
		}
	}

	void FixedUpdate()
	{
		Vector3 targetPosition = followTarget.transform.position;

		transform.position = Vector3.SmoothDamp(transform.position, targetPosition ,ref movementVelocity, movementDamping);
	}

	void LateUpdate () 
	{
		Quaternion targetRotation = Quaternion.LookRotation(followTarget.lookDirection,followTarget.transform.up);

		joint.rotation = targetRotation;
	}
}
