using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrink : MonoBehaviour {

	public float delay = 0;
	public float speed = 1.0f;

	private Vector3 initalScale;

	IEnumerator Start () 
	{
		initalScale = transform.localScale;

		yield return new WaitForSeconds(delay);

		float t = 1;
		while(t >= 0)
		{
			transform.localScale = initalScale * t;
			t -= Time.deltaTime * speed;
			yield return null;
		}

		transform.localScale = Vector3.zero;
	}
}
