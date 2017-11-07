using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSlider : MonoBehaviour {

	public Transform cam;

	[Tooltip("How fast should the camera adjust? (linear)")]
	public float speed = 1;
	[Tooltip("How close can the camera get to any wall?")]
	public float wallDistance = 0.1f;

	private Vector3 mLocalEndPosition;
	private float mMaxDistance;

	void OnEnable () {
		mLocalEndPosition = cam.localPosition;
		mMaxDistance = (transform.localPosition - mLocalEndPosition).magnitude;
	}

	private RaycastHit hit;

	void LateUpdate()
	{
//		if(Physics.Linecast(transform.position, transform.TransformPoint(localEndPosition), out hit))
		if(Physics.SphereCast(transform.position, wallDistance, transform.TransformPoint(mLocalEndPosition) - transform.position, out hit, mMaxDistance))
		{
			cam.position = Vector3.MoveTowards(cam.position,hit.point + hit.normal * wallDistance, speed);
//			Debug.DrawLine(transform.position, hit.point, Color.yellow);
		}
		else
		{
			cam.localPosition = Vector3.MoveTowards(cam.localPosition, mLocalEndPosition, speed * Time.deltaTime);
		}
	}

	void OnValidate()
	{
		wallDistance = Mathf.Clamp(wallDistance, 0.001f, Mathf.Infinity);
	}
}
