using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour 
{
	public Transform myCamera;
	public float defaultAmount = 10f;
	public float defaultDuration = 0.2f;

	public bool transpose = true;
	public bool rotate = false;
	public float smoothing = 5;

//	void Update () {
//		if(Input.GetKeyDown(KeyCode.P))
//		{
//			Shake();
//		}
//	}

	public void Shake()
	{
		Shake(defaultAmount, defaultDuration);
	}

	public void Shake(float amount, float duration)
	{
		StopAllCoroutines();
		StartCoroutine(C_Shake(amount, duration));
	}

	IEnumerator C_Shake (float amount, float duration) {

		while(duration > 0)
		{
			duration -= Time.deltaTime;

			Vector3 random = Random.insideUnitCircle * amount;
			if(transpose)
			{
				myCamera.localPosition = Vector3.Lerp(myCamera.localPosition, random * Mathf.Deg2Rad, Time.deltaTime * smoothing);
			}
			if(rotate)
			{
				myCamera.localRotation = Quaternion.Slerp(myCamera.localRotation, Quaternion.Euler(random), Time.deltaTime * smoothing);
			}
			yield return null;
		}

		while((transpose && myCamera.localPosition.sqrMagnitude > 0.00001f) || (rotate && Quaternion.Angle(myCamera.localRotation, Quaternion.identity) > 0.001f))
		{
			myCamera.localPosition = Vector3.Lerp(myCamera.localPosition, Vector3.zero, Time.deltaTime * smoothing);

			myCamera.localRotation = Quaternion.Slerp(myCamera.localRotation, Quaternion.identity, Time.deltaTime * smoothing);
			yield return null;
		}	

		myCamera.localPosition = Vector3.zero;
		myCamera.localRotation = Quaternion.identity;
	}
}
