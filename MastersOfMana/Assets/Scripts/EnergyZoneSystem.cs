using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnergyZoneSystem : NetworkBehaviour 
{
	public float duration = 20;

	[System.Serializable]
	public struct spawnZone
    {
		public float spawnDelay;
		public GameObject spanwObject;
	}

	public List<spawnZone> spawnZones;
	public Transform EnergyZones;

    public AudioSource audioSource;
    public AudioClip appearanceSfx;

    private List<Transform> mZoneSpawns = new List<Transform>();
    private int currentIndex = 0;

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
        mZoneSpawns.Clear();
        //find all possible spawnPositions
        for (int i = 0; i < EnergyZones.childCount; i++)
        {
            mZoneSpawns.Add(EnergyZones.GetChild(i));
        }
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
            Transform healSpawnPosition = mZoneSpawns.RandomElement();
            Vector3 position = healSpawnPosition.position;

            audioSource.transform.position = position;
            audioSource.Play();

            Quaternion rotation = healSpawnPosition.rotation;
            mZoneSpawns.Remove(healSpawnPosition);
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
        mZoneSpawns.Add(trans);
    }
}
