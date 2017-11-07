using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This components follows a transform with a certain amount of damping.
/// </summary>
public class PlayerCamera : MonoBehaviour {

	[Tooltip("This is the joint for the up and down rotation. This should be in the middle on the xz Plane, otherwise rotation will look weird")]
	public Transform joint;
	public PlayerScript followTarget;

	[Range(0,1)]
	public float movementDamping = .2f;

	public float shoulderSwapSpeed = 2;

	public float zoomedInFOV = 30;
	public float zoomSpeed = 1;

	private float mStartFOV;
	private int mCurrentShoulder = 1;
	private Camera mCamera;
	private Vector3 mMovementVelocity; 

	public Camera GetCamera()
	{
		return mCamera;
	}

    void Awake()
    {
		mCamera = GetComponentInChildren<Camera>();
		mStartFOV = mCamera.fieldOfView;
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

		transform.position = Vector3.SmoothDamp(transform.position, targetPosition ,ref mMovementVelocity, movementDamping);
	}

	void LateUpdate () 
	{
		Quaternion targetRotation = followTarget.aim.currentLookRotation;

		mCamera.fieldOfView = Mathf.MoveTowards(mCamera.fieldOfView, followTarget.aim.IsFocused() ? zoomedInFOV : mStartFOV, Time.deltaTime * zoomSpeed);

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

	/// <summary>
	/// Gets the center ray from the viewport.
	/// </summary>
	/// <returns>The center ray.</returns>
	public Ray GetCenterRay()
	{
		return mCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
	}

	/// <summary>
	/// Starts a coroutine that moves the camera from one shoulder to the other.
	/// </summary>
	public void SwapShoulder()
	{
		mCurrentShoulder *= -1;
		StopAllCoroutines();
		StartCoroutine(InterpolateShoulder());
	}

	private IEnumerator InterpolateShoulder()
	{
		Vector3 targetScale = new Vector3(mCurrentShoulder,1,1);
		float t = 0;
		while(t < 1)
		{
			joint.localScale = Vector3.Lerp(joint.localScale, targetScale, t);
			t += Time.deltaTime * shoulderSwapSpeed;
			yield return null;
		}
		joint.localScale = targetScale;
	}
}
