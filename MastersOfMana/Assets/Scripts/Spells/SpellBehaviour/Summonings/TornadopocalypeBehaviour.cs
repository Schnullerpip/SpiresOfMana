using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TornadopocalypeBehaviour : A_SummoningBehaviour 
{
	public Bounds bounds;

    public int amount = 8;

	public float duration = 15;

    public Tornado tornadoPrefab;

	private PlayerScript[] mOpponents;
	private PlayerScript mCaster;

    private Tornado[] tornados;

	#region implemented abstract members of A_SpellBehaviour
	public override void Execute (PlayerScript caster)
	{
		GameManager.instance.isUltimateActive = true;

        TornadopocalypeBehaviour tornadopocalypse = PoolRegistry.Instantiate(this.gameObject).GetComponent<TornadopocalypeBehaviour>();

		tornadopocalypse.mCaster = caster;
		tornadopocalypse.transform.position = transform.position;

		NetworkServer.Spawn(tornadopocalypse.gameObject, tornadopocalypse.GetComponent<NetworkIdentity>().assetId);
		tornadopocalypse.gameObject.SetActive(true);

		tornadopocalypse.Init();
	}
	#endregion

	private void Init()
	{
		PlayerScript[] allPlayers = GameObject.FindObjectsOfType<PlayerScript>();
		int j = 0;

		mOpponents = new PlayerScript[allPlayers.Length - 1];

		for (int i = 0; i < allPlayers.Length; ++i) 
		{
			if(allPlayers[i] == mCaster)
			{
				continue;
			}

			mOpponents[j] = allPlayers[i];
			++j;
		}

        tornados = new Tornado[amount];

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < amount; ++i)
        {
            tornados[i] = PoolRegistry.Instantiate(tornadoPrefab.gameObject).GetComponent<Tornado>();

            pos.x = Random.Range(-bounds.extents.x, bounds.extents.x);
            pos.z = Random.Range(-bounds.extents.z, bounds.extents.z);

            tornados[i].transform.position = pos;

            NetworkServer.Spawn(tornados[i].gameObject, tornados[i].GetComponent<NetworkIdentity>().assetId);
            tornados[i].gameObject.SetActive(true);
        }

        StartCoroutine(DelayedStop(duration));
	}

	private IEnumerator DelayedStop(float time)
	{
        yield return new WaitForSeconds(time);

        for (int i = 0; i < amount; ++i)
        {
            NetworkServer.UnSpawn(tornados[i].gameObject);
            tornados[i].gameObject.SetActive(false);
        }

		//reset the flag so a new ultimate can be started
		GameManager.instance.isUltimateActive = false;

		NetworkServer.UnSpawn(this.gameObject);
		this.gameObject.SetActive(false);
	}
}