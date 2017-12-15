using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealPackSystem : NetworkBehaviour {

    //public GameObject healPack;
	public float duration = 7;
    public List<float> spawnTimes = new List<float>();
	public List<GameObject> spawnObjects = new List<GameObject>();

    private List<Transform> mHealSpawns = new List<Transform>();
    private int currentIndex = 0;

    public void Start()
    {
        //find all possible spawnPositions
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("HealPackSpawn"))
        {
            mHealSpawns.Add(obj.transform);
        }
    }

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
		currentIndex = 0;
		StartCoroutine(SpawnHeals());
	}

	void RoundEnded()
	{
		StopAllCoroutines();
	}

    private IEnumerator SpawnHeals()
    {
        //Spawns the healthpackets one after the other
        while(currentIndex < spawnTimes.Count)
        {
            yield return new WaitForSeconds(spawnTimes[currentIndex]);

            //Get a random spawn position
			Transform healSpawnPosition = mHealSpawns[Mathf.FloorToInt(Random.Range(0, mHealSpawns.Count - 0.00001f))];
			Vector3 position = healSpawnPosition.position;
			Quaternion rotation = healSpawnPosition.rotation;
            mHealSpawns.Remove(healSpawnPosition);
			position.y += spawnObjects[currentIndex].transform.localScale.y * 0.5f;
            //Instantiate and spawn
			GameObject obj = Instantiate(spawnObjects[currentIndex], position, rotation);
			StartCoroutine(Despawn(healSpawnPosition, obj));
            NetworkServer.Spawn(obj.gameObject);
			currentIndex++;
        }
    }

    private IEnumerator Despawn(Transform trans, GameObject obj)
    {
		yield return new WaitForSeconds(duration);
		NetworkServer.UnSpawn (obj);
		Destroy (obj);
        mHealSpawns.Add(trans);
    }
}
