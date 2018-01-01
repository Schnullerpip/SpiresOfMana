using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnergyZoneSystem : NetworkBehaviour 
{
    public float initialDelay = 10;
	public float lifetime = 20;
    public float gapBetweenSpawns = 10;
    public List<GameObject> spawnables;
    public Transform spawnpointsParent;

	//[System.Serializable]
	//public struct spawnZone
 //   {
	//	public float spawnDelay;
	//	public GameObject spawnObject;
	//}

    private List<Transform> mSpawnpoints = new List<Transform>();

	public void OnEnable()
	{
		GameManager.OnRoundStarted += RoundStarted;
		GameManager.OnRoundEnded += RoundEnded;
	}

	public void OnDisable()
	{
		StopAllCoroutines();
		GameManager.OnRoundStarted -= RoundStarted;
		GameManager.OnRoundEnded -= RoundEnded;
	}

	void RoundStarted()
	{
        mSpawnpoints.Clear();
        //find all possible spawnPoints
        for(int i = 0; i < spawnpointsParent.childCount; i++)
        {
            Transform child = spawnpointsParent.GetChild(i);
            if (child.gameObject.activeSelf)
                mSpawnpoints.Add(child);
        }

        StartCoroutine(SpawnHeals(true));
	}

	void RoundEnded()
	{
		StopAllCoroutines();
	}

    private IEnumerator SpawnHeals(bool init = false)
    {
        if(init)
            yield return new WaitForSeconds(initialDelay);//to skip initial spawn; is 1 frame "too late"
        else
            yield return new WaitForSeconds(gapBetweenSpawns);

        //Get a random spawn position
        Transform healSpawnPosition = mSpawnpoints.RandomElement();
        Vector3 position = healSpawnPosition.position;
        Quaternion rotation = healSpawnPosition.rotation;
        //Instantiate and spawn
        GameObject obj = Instantiate(spawnables.RandomElement(), position, rotation);
        obj.transform.localScale = healSpawnPosition.localScale;
        StartCoroutine(Despawn(healSpawnPosition, obj));
        NetworkServer.Spawn(obj.gameObject);
    }

    private IEnumerator Despawn(Transform trans, GameObject obj)
    {
		yield return new WaitForSeconds(lifetime);
		NetworkServer.UnSpawn (obj);
		Destroy (obj);
        StartCoroutine(SpawnHeals());
    }
}
