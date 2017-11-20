using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PreviewSpell : MonoBehaviour 
{
	public float smoothTime = 0.016f;
	protected Vector3 mDesiredPos;
	protected Quaternion mDesiredRot;

	private PreviewSpell mPreview;

	/// <summary>
	/// Gets the scene instance of the preview object. Instatiates a new one if null
	/// </summary>
	/// <value>The instance.</value>
	public PreviewSpell instance
	{
		get
		{
			if(!mPreview)
			{
				mPreview = GameObject.Instantiate(this);
			}
			return mPreview;
		}
	}

	void Awake()
	{
		Deactivate();
	}

	/// <summary>
	/// Deactivate this instance.
	/// </summary>
	public void Deactivate()
	{
		#if UNITY_EDITOR
		Assert.IsFalse(IsPrefab(), "Method called on Prefab. Use instance instead!");
		#endif
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Moves the position and rotates. Also activates the object if inactive.
	/// Use this instead of Move() and Rotate() seperately to avoid unnecessary SetActive() calls
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="rotation">Rotation.</param>
	public void MoveAndRotate(Vector3 position, Quaternion rotation)
	{
		#if UNITY_EDITOR
		Assert.IsFalse(IsPrefab(), "Method called on Prefab. Use instance instead!");
		#endif		
		mDesiredPos = position;
		mDesiredRot = rotation;

		if(!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
			transform.position = mDesiredPos;
			transform.rotation = mDesiredRot;
		}
	}

	/// <summary>
	/// Move the specified position. Also activates the object if inactive.
	/// </summary>
	/// <param name="position">Position.</param>
	public void Move(Vector3 position)
	{
		#if UNITY_EDITOR
		Assert.IsFalse(IsPrefab(), "Method called on Prefab. Use instance instead!");
		#endif		
		mDesiredPos = position;

		if(!gameObject.activeInHierarchy)
		{
			gameObject.SetActive(true);
			transform.position = mDesiredPos;
		}
	}

	/// <summary>
	/// Rotate the specified rotation. Also activates the object if inactive.
	/// </summary>
	/// <param name="rotation">Rotation.</param>
	public void Rotate(Quaternion rotation)
	{
		#if UNITY_EDITOR
		Assert.IsFalse(IsPrefab(), "Method called on Prefab. Use instance instead!");
		#endif
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

	#if UNITY_EDITOR
	private bool IsPrefab()
	{
		return UnityEditor.PrefabUtility.GetPrefabParent(gameObject) == null && UnityEditor.PrefabUtility.GetPrefabObject(gameObject) != null;
	}
	#endif
}
