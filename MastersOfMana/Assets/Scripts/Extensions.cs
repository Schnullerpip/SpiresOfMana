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


}