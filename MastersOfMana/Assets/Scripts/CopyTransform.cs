using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransform : MonoBehaviour 
{
	public Transform copyTarget;

	public bool position = false;
	public bool rotation = false;
	public bool scale = false;

	void LateUpdate () 
	{
		if(position)
		{
			transform.position = copyTarget.position;
		}
		if(rotation)
		{
			transform.rotation = copyTarget.rotation;
		}
		if(scale)
		{
			transform.localScale = copyTarget.localScale;
		}
	}
}
