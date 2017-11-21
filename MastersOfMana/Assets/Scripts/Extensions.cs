using UnityEngine;
using System.Collections;

public static class Extensions
{
	/// <summary>
	/// Returns a Vector2 with the x and z Component of the 
	/// original Vector3 as x and y respectively.
	/// </summary>
	/// <param name="vec3">Vec3.</param>
	public static Vector2 xz(this Vector3 vec3)
	{
		return new Vector2(vec3.x,vec3.z);
	}

	#region Transform

	/// <summary>
	/// Resets the transformation, including position, 
	/// localRotaion and localScale.
	/// </summary>
	/// <param name="trans">Trans.</param>
	public static void ResetTransformation(this Transform trans)
	{
		trans.position = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = Vector3.one;
	}

	public static void SetLayer(this Transform trans, int layer, bool recursive = true) 
	{
		trans.gameObject.layer = layer;
		if(recursive)
		{
			foreach(Transform child in trans)
			{
				child.SetLayer(layer, true);
			}
		}		
	}

	#endregion

	#region Rigidbody

	/// <summary>
	/// Resets the velocity and angular velcity of the specified rigidbody.
	/// </summary>
	/// <param name="rigid">Rigid.</param>
	public static void Reset(this Rigidbody rigid)
	{
		rigid.velocity = Vector3.zero;
		rigid.angularVelocity = Vector3.zero;
	}

	/// <summary>
	/// Overrides only the y component of the velocity.
	/// </summary>
	/// <param name="y">The y component.</param>
	public static void SetVelocityY(this Rigidbody rigid, float y)
	{
		rigid.velocity = new Vector3(rigid.velocity.x, y, rigid.velocity.z);
	}

	#endregion

	#region Quaternion
	/// <summary>
	/// Gradually changes a value towards a desired goal over time.
	/// The quaternion is smoothed by some spring-damper like function, which will never overshoot.
	/// </summary>
	/// <returns>The damped quaternion.</returns>
	/// <param name="current">The current quaternion.</param>
	/// <param name="target">The quaternion we are trying to reach.</param>
	/// <param name="currentDerivative">The current derivative, this value is modified by the function every time you call it.</param>
	/// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
	/// <param name="deltaTime">The time since the last call to this function.</param>
	public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Quaternion currentDerivative, float smoothTime, float deltaTime) 
	{
		// account for double-cover
		float dot = Quaternion.Dot(current, target);
		float f = dot > 0f ? 1f : -1f;
		target.x *= f;
		target.y *= f;
		target.z *= f;
		target.w *= f;
		// smooth damp (nlerp approx)
		Vector4 result = new Vector4(
			Mathf.SmoothDamp(current.x, target.x, ref currentDerivative.x, smoothTime),
			Mathf.SmoothDamp(current.y, target.y, ref currentDerivative.y, smoothTime),
			Mathf.SmoothDamp(current.z, target.z, ref currentDerivative.z, smoothTime),
			Mathf.SmoothDamp(current.w, target.w, ref currentDerivative.w, smoothTime)
		).normalized;
		// compute deriv
		var dtInv = 1f / deltaTime;
		currentDerivative.x = (result.x - current.x) * dtInv;
		currentDerivative.y = (result.y - current.y) * dtInv;
		currentDerivative.z = (result.z - current.z) * dtInv;
		currentDerivative.w = (result.w - current.w) * dtInv;
		return new Quaternion(result.x, result.y, result.z, result.w);
	}

	/// <summary>
	/// Gradually changes a value towards a desired goal over time.
	/// The quaternion is smoothed by some spring-damper like function, which will never overshoot.
	/// </summary>
	/// <returns>The damped quaternion.</returns>
	/// <param name="current">The current quaternion.</param>
	/// <param name="target">The quaternion we are trying to reach.</param>
	/// <param name="currentDerivative">The current derivative, this value is modified by the function every time you call it.</param>
	/// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
	public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Quaternion currentDerivative, float smoothTime) 
	{
		return SmoothDamp(current, target, ref currentDerivative, smoothTime, Time.deltaTime);
	}
	#endregion
}