using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCamera : MonoBehaviour {

	public bool findPlayer = true;
	public Transform target;
	public Vector3 offset = new Vector3(0,-3,0);
	public float speed;

	void LateUpdate () 
	{
		if(!target && findPlayer)
			target = GameObject.FindGameObjectWithTag("Player").transform;

		if(!target)
			return;
		
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position + offset - transform.position), Time.deltaTime * speed);
	}
}
