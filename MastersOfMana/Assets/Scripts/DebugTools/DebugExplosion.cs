using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugExplosion : MonoBehaviour 
{
	public KeyCode key;
	public float force = 100;
	public float radius = 1;

	void Update()
	{
		if(Input.GetKeyDown(key))
		{
			Collider[] cols;
			cols = Physics.OverlapSphere(transform.position,radius);

			foreach (var c in cols) 
			{
				if(c.attachedRigidbody)
				{
					c.attachedRigidbody.AddExplosionForce(force,transform.position,radius);
				}
			}
		}
	}
}
