using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PoolRegistry : NetworkBehaviour {

    public static PoolRegistry instance;
    public int defaultPoolSize;
    public bool poolShouldGrow;

    public List<Pool> poolList = new List<Pool>();

    public void Start()
    {
        if(instance == null)
        {
            Debug.Log("PoolRegistry created!");
            instance = this;
        }

        if (isServer)
        {
            GameManager.instance.Go();
        }
    }

    public GameObject Instantiate(GameObject go)
    {
        if (poolList.Count > 0)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (poolList[i].mOriginal == go)
                {
                    return poolList[i].Get();
                }
            }
        }

        Pool newPool = new Pool(go, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
        poolList.Add(newPool);

        return newPool.Get();
    }
}
