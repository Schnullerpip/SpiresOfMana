using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelfDeactivateAndUnspawn : NetworkBehaviour {

	public float lifetime = 1;

	IEnumerator Start () 
	{
		yield return new WaitForSeconds(lifetime);
        this.gameObject.SetActive(false);
        NetworkServer.UnSpawn(this.gameObject);
	}
}
