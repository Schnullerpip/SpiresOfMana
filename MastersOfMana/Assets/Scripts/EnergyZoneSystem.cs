using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnergyZoneSystem : NetworkBehaviour 
{
    public float initialDelay = 10;
	public float lifetime = 20;
    public float gapBetweenSpawns = 10;
    //public List<GameObject> spawnables;
    public List<spawnZone> spawnables;
    public Transform spawnpointsParent;

    [System.Serializable]
    public struct spawnZone
    {
        public GameObject prefab;
        [Range(0.0f, 1.0f)]
        public float randomFactor;
    }

    private List<PlatformSpiresEffect> mSpawnpoints = new List<PlatformSpiresEffect>();
    private float factorSum;

    public void Start()
    {
        spawnables.Sort((s1, s2) => s1.randomFactor.CompareTo(s2.randomFactor));//sorts the list so the object with the lowest random factor is first
        factorSum = 0.0f;
        foreach (var s in spawnables)
        {
            factorSum += s.randomFactor;
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
        mSpawnpoints.Clear();
        //find all possible spawnPoints
        for(int i = 0; i < spawnpointsParent.childCount; i++)
        {
            Transform child = spawnpointsParent.GetChild(i);
            if (child.gameObject.activeSelf)
                mSpawnpoints.Add(child.GetComponent<PlatformSpiresEffect>());
        }

        StartCoroutine(SpawnZone(true));
	}

	void RoundEnded()
	{
        for (int i = 0; i < mSpawnpoints.Count; ++i)
        {
            mSpawnpoints[i].RpcDeactivate();
        }
        StopAllCoroutines();
	}

    private IEnumerator SpawnZone(bool init = false)
    {
        if(init)
            yield return new WaitForSeconds(initialDelay);//to skip initial spawn; is 1 frame "too late"
        else
            yield return new WaitForSeconds(gapBetweenSpawns);

        //Get a random spawn position
        PlatformSpiresEffect platform = GetRandomPlatform();
        Transform spawnPoint = platform.transform;
        Vector3 position = spawnPoint.position;
        Quaternion rotation = spawnPoint.rotation;

        GameObject objToSpawn = null;
        float r = Random.value,
              rCnt = 0.0f;
        foreach(var s in spawnables)
        {
            rCnt += s.randomFactor / factorSum;
            if (r < rCnt)
            {
                objToSpawn = s.prefab;
                break;
            }
        }

        if(objToSpawn == null)
        {
            Debug.LogError("Couldn't spawn loading zone, because it was impossible to determine which should get spawned.");
            yield break;
        }

        //Instantiate and spawn
        GameObject obj = Instantiate(objToSpawn, position, rotation);
        //obj.transform.localScale = spawnPoint.localScale;
        obj.GetComponent<LoadingZone>().spawnScale = spawnPoint.localScale;
        StartCoroutine(Despawn(spawnPoint, obj, platform));
        NetworkServer.Spawn(obj);

        platform.RpcActivate(obj.GetComponent<SetEffectColor>().Color);
    }

    private PlatformSpiresEffect GetRandomPlatform()
    {
        PlatformSpiresEffect spawnPlatform = mSpawnpoints.RandomElement();

        if (spawnPlatform.transform.position.y < GameManager.instance.GetLavaFloorHeight())
        {
            mSpawnpoints.Remove(spawnPlatform);
            spawnPlatform = GetRandomPlatform();
        }
        return spawnPlatform;
    }

    private IEnumerator Despawn(Transform trans, GameObject obj, PlatformSpiresEffect platform)
    {
		yield return new WaitForSeconds(lifetime);
		NetworkServer.UnSpawn (obj);
		Destroy (obj);
        StartCoroutine(SpawnZone());

        platform.RpcDeactivate();
    }
}
