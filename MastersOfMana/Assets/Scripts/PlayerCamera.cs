using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This components follows a transform with a certain amount of damping.
/// </summary>
public class PlayerCamera : MonoBehaviour {

	public PlayerScript followTarget;
	public bool keepOffset;

	[Range(0,1)]
	public float movementDamping = .2f;

	[ReadOnlyAttribute]
	public Vector3 movementVelocity; 

	private Vector3 mOffset;

	void Awake()
	{
		if(followTarget == null)
		{
			Debug.LogWarning("No Follow Target assigned. Script is turned off",this.gameObject);
			enabled = false;
		}

		mOffset = transform.position - followTarget.transform.position;
	}

	void FixedUpdate()
	{
		Vector3 targetPosition = followTarget.transform.position + (keepOffset ? mOffset : Vector3.zero);

		transform.position = Vector3.SmoothDamp(transform.position, targetPosition ,ref movementVelocity, movementDamping);
	}

	void LateUpdate () 
	{
		Quaternion targetRotation = Quaternion.LookRotation(followTarget.lookDirection,followTarget.transform.up);

		transform.rotation = targetRotation;

	}
}
