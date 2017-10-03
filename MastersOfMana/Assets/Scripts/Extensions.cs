using UnityEngine;
using System.Collections;

public static class Extensions
{
	/// <summary>
	/// Returns a Vector2 with the x and z Component of the 
	/// original Vector3 as x and y respectiply.
	/// </summary>
	/// <param name="vec3">Vec3.</param>
	public static Vector2 xz(this Vector3 vec3)
	{
		return new Vector2(vec3.x,vec3.z);
	}

	/// <summary>
	/// Resets the transformation, including position, 
	/// localRotaion and localScale.
	/// </summary>
	/// <param name="trans">Trans.</param>
	public static void ResetTransformation(this Transform trans)
	{
		trans.position = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = new Vector3(1, 1, 1);
	}

	/// <summary>
	/// Reset the velocuty and angular velcity of the specified rigidbody.
	/// </summary>
	/// <param name="rigid">Rigid.</param>
	public static void Reset(this Rigidbody rigid)
	{
		rigid.velocity = Vector3.zero;
		rigid.angularVelocity = Vector3.zero;
	}
}