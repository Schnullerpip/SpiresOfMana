using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    #region Vector
    /// <summary>
    /// Returns a Vector2 with the x and z Component of the 
    /// original Vector3 as x and y respectively.
    /// </summary>
    /// <param name="vec3">Vec3.</param>
    public static Vector2 xz(this Vector3 vec3)
	{
		return new Vector2(vec3.x,vec3.z);
	}

    public static Vector3 ToVector3(this Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y, 0);
    }

    public static Vector3 ToVector3xz(this Vector2 vec2)
    {
        return new Vector3(vec2.x, 0, vec2.y);
    }

    /// <summary>
    /// <para>Returns the specified vec3 with a changed x, y and/or z component.
    /// Can be used if the temporary vector is not necessary.</para>
    /// <para>eg: transform.position = transform.position.Change( x: 10 );</para>
    /// </summary>
    /// <returns>The change.</returns>
    /// <param name="vec3">Vec3.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    public static Vector3 Change(this Vector3 vec3, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vec3.x, y ?? vec3.y, z ?? vec3.z);
    }
    #endregion

	#region IList
	/// <summary>
    /// Returns a random element of the list.
	/// </summary>
	/// <returns>The element.</returns>
	public static T RandomElement<T>(this IList<T> list)
	{
        return list[list.RandomIndex()];
	}

	/// <summary>
    /// Returns a random index of the list.
	/// </summary>
	/// <returns>The index.</returns>
    public static int RandomIndex<T>(this IList<T> list)
	{
        return UnityEngine.Random.Range(0,list.Count);
	}

    /// <summary>
    /// Returns the last element of the list.
    /// </summary>
    /// <returns>The element.</returns>
    /// <param name="list">List.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T LastElement<T>(this IList<T> list)
    {
        return list[list.Count - 1];
    }

    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this List<T> ts)
    {
        for (int i = 0; i < ts.Count - 1; ++i)
        {
            var r = UnityEngine.Random.Range(i, ts.Count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
	#endregion

    #region Bounds
    public static Vector3 RandomInside(this Bounds bounds)
    {
        Vector3 returnVec;
        returnVec.x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        returnVec.y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
        returnVec.z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        return returnVec;
    }
    #endregion

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

    public static void SetPosition(this Transform trans, float? x = null, float? y = null, float? z = null)
    {
        trans.position = new Vector3(x ?? trans.position.x, y ?? trans.position.y, z ?? trans.position.z);
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
    #region HelperFunctions
    /// <summary>
    /// Converts from a normalized linear value (from 0 to 1) to a decibel value (from -80dB to 0dB)
    /// </summary>
    /// <returns>The decibel value.</returns>
    /// <param name="linear">Linear value.</param>
    public static float LinearToDecibel(float linear)
    {
        return linear > Mathf.Epsilon ? 20.0f * Mathf.Log10(linear) : -80.0f;
    }

    /// <summary>
    /// Converts from decibel (from -80dB to 0dB) to a normalized linear value (from 0 to 1)
    /// </summary>
    /// <returns>The linear value.</returns>
    /// <param name="dB">Decibel value.</param>
    public static float DecibelToLinear(float dB)
    {
        return Mathf.Pow(10.0f, dB / 20.0f);
    }
    #endregion
}