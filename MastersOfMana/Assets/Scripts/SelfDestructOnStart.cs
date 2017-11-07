using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructOnStart : MonoBehaviour {

	public float lifetime = 1;

	IEnumerator Start () 
	{
		yield return new WaitForSeconds(lifetime);
		Destroy(this.gameObject);
	}
}
