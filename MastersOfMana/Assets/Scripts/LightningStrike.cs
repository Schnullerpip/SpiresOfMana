using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LightningStrike : NetworkBehaviour
{
	public GameObject blobShadow;
	public GameObject line;
	public float lifetime = 0.5f;

	public float anticipationTime = 1;

	void OnEnable()
	{
		StartCoroutine(Flash());
	}

	IEnumerator Flash()
	{
		line.SetActive(false);

		blobShadow.SetActive(true);

		yield return new WaitForSeconds(anticipationTime);

		blobShadow.SetActive(false);
		line.SetActive(true);

		yield return new WaitForSeconds(lifetime);

		line.SetActive(false);

		NetworkServer.UnSpawn(this.gameObject);
		this.gameObject.SetActive(false);
	}

}
