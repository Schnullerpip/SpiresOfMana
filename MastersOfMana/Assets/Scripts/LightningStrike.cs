using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LightningStrike : NetworkBehaviour
{
	public GameObject blobShadow;
	public GameObject line;
	public float lifetime = 0.5f;
    private float mCurrentLifetime;

	public float anticipationTime = 1;

    public PlayerScript target;

	void OnEnable()
	{
        if(target)
        {
            transform.position = target.movement.GetAnticipationPosition(1);
        }
        mCurrentLifetime = lifetime;
		StartCoroutine(Flash());
	}

    private void Update()
    {
        if(isServer)
        {
            if(mCurrentLifetime > lifetime * 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 2 * Time.deltaTime);
            }

            mCurrentLifetime -= Time.deltaTime;
		}
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
