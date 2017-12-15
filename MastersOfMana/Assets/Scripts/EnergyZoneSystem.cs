using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnergyZoneSystem : NetworkBehaviour {

	public float duration = 20;

	[System.Serializable]
	public struct spawnZone{
		public float spawnDelay;
		public GameObject spanwObject;
	}

	public List<spawnZone> spawnZones;
	public Transform EnergyZones;

    private List<Transform> mHealSpawns = new List<Transform>();
    private int currentIndex = 0;

    public void Start()
    {
        //find all possible spawnPositions
		for(int i = 0; i < EnergyZones.childCount; i++)
        {
			mHealSpawns.Add(EnergyZones.GetChild(i));
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
		while(currentIndex < spawnZones.Count)
        {
			yield return new WaitForSeconds(spawnZones[currentIndex].spawnDelay);

            //Get a random spawn position
			Transform healSpawnPosition = mHealSpawns[Mathf.FloorToInt(Random.Range(0, mHealSpawns.Count - 0.00001f))];
			Vector3 position = healSpawnPosition.position;
			Quaternion rotation = healSpawnPosition.rotation;
            mHealSpawns.Remove(healSpawnPosition);
			position.y += spawnZones[currentIndex].spanwObject.transform.localScale.y * 0.5f;
            //Instantiate and spawn
			GameObject obj = Instantiate(spawnZones[currentIndex].spanwObject, position, rotation);
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
