using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealPackSystem : NetworkBehaviour {

    public GameObject healPack;
    public List<float> spawnTimes = new List<float>();

    private List<Transform> mHealSpawns = new List<Transform>();
    private int currentIndex = 0;

    public void Start()
    {
        //find all possible spawnPositions
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("HealPackSpawn"))
        {
            mHealSpawns.Add(obj.transform);
        }
        StartCoroutine(SpawnHeals());
    }

    private IEnumerator SpawnHeals()
    {
        //Spawns the healthpackets one after the other
        while(currentIndex < spawnTimes.Count)
        {
            yield return new WaitForSeconds(spawnTimes[currentIndex]);
            currentIndex++;

            //Get a randome spawn position
            Transform healSpawnPosition = mHealSpawns[Mathf.FloorToInt(Random.Range(0, mHealSpawns.Count - 0.00001f))];
            mHealSpawns.Remove(healSpawnPosition);

            //Instantiate and spawn
            GameObject obj = Instantiate(healPack, healSpawnPosition.position, healSpawnPosition.rotation);
            obj.GetComponent<HealPack>().healSpawnCallback = AddHealSpawn;
            NetworkServer.Spawn(obj);
        }
    }

    public void AddHealSpawn(Transform trans)
    {
        mHealSpawns.Add(trans);
    }
}
