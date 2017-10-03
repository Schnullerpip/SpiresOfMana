using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A very simple component that only rotates the object in any axis.
/// </summary>
public class AutoRotate : MonoBehaviour {

	[Tooltip("In Degree per second")]
	public Vector3 axis;

	public bool useUnscaledTime = false;

	void Update () 
	{
		float t = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
		transform.Rotate(axis*t);
	}
}
