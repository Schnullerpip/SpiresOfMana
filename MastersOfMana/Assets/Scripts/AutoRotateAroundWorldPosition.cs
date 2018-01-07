using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateAroundWorldPosition : MonoBehaviour {

    public Vector3 pivot;
    public Vector3 axis = Vector3.up;
    public float speed;

	void Update () 
    {
		transform.RotateAround(pivot, axis, speed * Time.deltaTime);
	}

    private void OnValidate()
    {
        axis = Vector3.Normalize(axis);
    }
}
