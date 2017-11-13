using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewDebug : MonoBehaviour 
{
	public float smoothTime = 0.16f;
	private Vector3 desiredPos;

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void Move(Vector3 position)
	{
		if(!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
			transform.position = position;
		}
		desiredPos = position;
	}

	Vector3 vel;

	void LateUpdate()
	{
		transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref vel, smoothTime);
	}
}
