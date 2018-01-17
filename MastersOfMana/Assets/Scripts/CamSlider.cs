using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSlider : MonoBehaviour {

	public Transform cam;

	[Tooltip("How fast should the camera adjust if something is in the way? (linear)")]
	public float inSpeed = 10;
	[Tooltip("How fast should the camera adjust back to its correct position? (linear)")]
	public float outSpeed = 5;
	[Tooltip("How close can the camera get to any wall?")]
	public float wallDistance = 0.1f;

	private Vector3 mLocalEndPosition;
	private float mMaxDistance;
	private RaycastHit mHit;
	private PlayerScript mPlayer;

	void OnEnable () {
		mLocalEndPosition = cam.localPosition;
		mMaxDistance = (transform.localPosition - mLocalEndPosition).magnitude;
	}

	public void SetPlayer(PlayerScript player)
	{
		mPlayer = player;
	}

	void LateUpdate()
	{
        if (mPlayer)
        {
            //disable collision with player
            mPlayer.SetColliderIgnoreRaycast(true);

            //first check if the camera is already touching a collider
            if(Physics.CheckSphere(transform.position, wallDistance))
            {
                cam.localPosition = Vector3.MoveTowards(cam.localPosition, Vector3.zero, inSpeed * Time.deltaTime);
            }
            //then check with a spherecast backwards if there is a wall
            else if (Physics.SphereCast(transform.position, wallDistance, transform.TransformPoint(mLocalEndPosition) - transform.position, out mHit, mMaxDistance))
            {
                cam.position = Vector3.MoveTowards(cam.position, mHit.point + mHit.normal * wallDistance, inSpeed * Time.deltaTime);
            }
            //if not, no sliding necessary, move back to original position;
            else
            {
                cam.localPosition = Vector3.MoveTowards(cam.localPosition, mLocalEndPosition, outSpeed * Time.deltaTime);
            }
            //reenable collision with player
            mPlayer.SetColliderIgnoreRaycast(false);
        }
	}

	void OnValidate()
	{
		wallDistance = Mathf.Clamp(wallDistance, 0.001f, Mathf.Infinity);
	}
}
