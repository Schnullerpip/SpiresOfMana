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

	/// <summary>
	/// Shakes the camera with a scaling amount and the default duration.
	/// </summary>
	/// <param name="dmg">Dmg.</param>
	public void ShakeByDamage(int dmg)
	{
		Shake(dmg * 1.0f, defaultDuration);
	}

    /// <summary>
    /// Shakes the camera with a scaling amount and the default duration.
    /// </summary>
    /// <param name="dmg">Dmg.</param>
    public void ShakeByDamage(float dmg)
    {
        Shake(dmg, defaultDuration);
    }

	/// <summary>
	/// Shake the camera with the default amount and duration.
	/// </summary>
	public void Shake()
	{
		Shake(defaultAmount, defaultDuration);
	}
		
	/// <summary>
	/// Shake the camera with specified amount and duration.
	/// </summary>
	/// <param name="amount">Amount.</param>
	/// <param name="duration">Duration.</param>
	public void Shake(float amount, float duration)
	{
		StopAllCoroutines();
        if(isActiveAndEnabled)
        {
            StartCoroutine(C_Shake(amount, duration));
        }
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

    public void OnDisable()
    {
        StopAllCoroutines();
    }
}
