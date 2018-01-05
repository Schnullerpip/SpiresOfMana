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

    private List<PlatformSpiresEffect> mSpawnpoints = new List<PlatformSpiresEffect>();
    private LavaFloor mLavaFloor;

    public void Start()
    {
        mLavaFloor = FindObjectOfType<LavaFloor>();
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
        //Instantiate and spawn
        GameObject obj = Instantiate(spawnables.RandomElement(), position, rotation);
        //obj.transform.localScale = spawnPoint.localScale;
        obj.GetComponent<LoadingZone>().spawnScale = spawnPoint.localScale;
        StartCoroutine(Despawn(spawnPoint, obj, platform));
        NetworkServer.Spawn(obj);

        platform.RpcActivate(obj.GetComponent<SetEffectColor>().Color);
    }

    private PlatformSpiresEffect GetRandomPlatform()
    {
        PlatformSpiresEffect healSpawnPlatform = mSpawnpoints.RandomElement();

        if (healSpawnPlatform.transform.position.y < mLavaFloor.transform.position.y)
        {
            mSpawnpoints.Remove(healSpawnPlatform);
            healSpawnPlatform = GetRandomPlatform();
        }
        return healSpawnPlatform;
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
