using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
/// <summary>
/// Lukas laboratory. This class is only here for various testing purposes, do not use it. For realsies!
/// </summary>
public class LukasLaboratory : MonoBehaviour {

	public Vector2 a;
	public Vector2 b;

	public float angle;

	void OnEnable()
	{
		Debug.LogWarning("DELETE THIS!",this);
	}

	// Update is called once per frame
	void Update () 
	{
		angle = Vector2.SignedAngle(a,b);
	}
}
