using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This components follows a transform with a certain amount of damping.
/// </summary>
public class SmoothFollow : MonoBehaviour {

	public Transform followTarget;

	[Range(0,1)]
	public float movementDamping = .2f;

	[Range(0,1)]
	public float rotationDamping = .2f;

	[ReadOnlyAttribute]
	public Vector3 movementVelocity; 
	[ReadOnlyAttribute]
	public Vector3 rotationVelocity;

	void LateUpdate () 
	{
		transform.position = Vector3.SmoothDamp(transform.position,followTarget.position,ref movementVelocity, movementDamping);
		transform.rotation = Quaternion.LookRotation(Vector3.SmoothDamp(transform.forward, followTarget.forward, ref rotationVelocity, rotationDamping),transform.up);
	}
}
