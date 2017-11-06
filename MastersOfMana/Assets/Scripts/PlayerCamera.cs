using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This components follows a transform with a certain amount of damping.
/// </summary>
public class PlayerCamera : MonoBehaviour {

	public Transform joint;
	public PlayerScript followTarget;

	[Range(0,1)]
	public float movementDamping = .2f;

	public Vector3 movementVelocity; 
	public float zoomedInFOV = 30;
	public float zoomSpeed = 1;

	private float startFOV;

	private Vector3 mOffset;

	private Camera mCamera;

	public Camera GetCamera()
	{
		return mCamera;
	}

    void Awake()
    {
		mCamera = GetComponentInChildren<Camera>();
		startFOV = mCamera.fieldOfView;
    }

	void Start()
	{
		if(followTarget == null)
		{
			Debug.LogWarning("No Follow Target assigned.", this.gameObject);
		}
	}

	void FixedUpdate()
	{
		Vector3 targetPosition = followTarget.transform.position;

		transform.position = Vector3.SmoothDamp(transform.position, targetPosition ,ref movementVelocity, movementDamping);
	}

	void LateUpdate () 
	{
		Quaternion targetRotation = followTarget.aim.currentLookRotation;

		if(followTarget.aim.IsFocused())
		{
			mCamera.fieldOfView = Mathf.MoveTowards(mCamera.fieldOfView, zoomedInFOV, Time.deltaTime * zoomSpeed);
		}
		else
		{
			mCamera.fieldOfView = Mathf.MoveTowards(mCamera.fieldOfView, startFOV, Time.deltaTime * zoomSpeed);
		}
		joint.rotation = targetRotation;
	}

	/// <summary>
	/// Shoots a ray from the center of the viewport.
	/// </summary>
	/// <returns><c>true</c>, if raycast hit something, <c>false</c> otherwise.</returns>
	/// <param name="hit">Hit.</param>
	public bool CenterRaycast(out RaycastHit hit)
	{
		return Physics.Raycast(GetCenterRay(), out hit);
	}

	public Ray GetCenterRay()
	{
		return mCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
	}

	/// <summary>
	/// Shoots a ray from the 3D position of the camera, offset by its approx. distance to the players head.
	/// </summary>
	/// <returns><c>true</c>, if the raycast hit, <c>false</c> otherwise.</returns>
	/// <param name="position">Position.</param>
	/// <param name="hit">Hit.</param>
	public bool RaycastCheck(Vector3 position, out RaycastHit hit)
	{
		Vector3 rayStart = mCamera.transform.position + mCamera.transform.forward * (-mCamera.transform.localPosition.z + .5f);
		Vector3 direction = position - rayStart;
		return Physics.Raycast(rayStart, direction, out hit);
	}

	public void SwapShoulder()
	{
		Debug.Log("Shoulder Swapping not implemented yet");
		return;

//		Vector3 camPos = mCamera.transform.localPosition;
//		camPos.x *= -1;
//		mCamera.transform.localPosition = camPos;
	}
}
