using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSpell : MonoBehaviour 
{
	public float smoothTime = 0.016f;
	protected Vector3 mDesiredPos;
	protected Quaternion mDesiredRot;

	void Awake()
	{
		Deactivate();
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void MoveAndRotate(Vector3 position, Quaternion rotation)
	{
		mDesiredPos = position;
		mDesiredRot = rotation;

		if(!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
			transform.position = mDesiredPos;
			transform.rotation = mDesiredRot;
		}
	}

	public void Move(Vector3 position)
	{
		mDesiredPos = position;

		if(!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
			transform.position = mDesiredPos;
		}
	}

	public void Rotate(Quaternion rotation)
	{
		mDesiredRot = rotation;

		if(!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
			transform.rotation = mDesiredRot;
		}
	}

	Vector3 vel;
	Quaternion rot;

	void LateUpdate()
	{
		transform.position = Vector3.SmoothDamp(transform.position, mDesiredPos, ref vel, smoothTime);
		transform.rotation = Extensions.SmoothDamp(transform.rotation, mDesiredRot, ref rot, smoothTime);
	}
}
