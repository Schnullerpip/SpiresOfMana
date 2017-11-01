using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballExplosion : MonoBehaviour {

	public float lifetime = 1;

	IEnumerator Start () 
	{
		yield return new WaitForSeconds(1);
		Destroy(this.gameObject);
	}
}
