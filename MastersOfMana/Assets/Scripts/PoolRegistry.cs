using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PoolRegistry : NetworkBehaviour {

    public static PoolRegistry instance;
    public int defaultPoolSize;
    public Pool.PoolingStrategy poolingStrategy;

    public List<Pool> poolList = new List<Pool>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Start()
    {
        if (isServer)
        {
            GameManager.instance.Go();
        }
    }

    public static GameObject Instantiate(GameObject go, Pool.Activation activationState = Pool.Activation.ReturnDeactivated)
    {
        if (instance.poolList.Count > 0)
        {
            for (int i = 0; i < instance.poolList.Count; i++)
            {
                if (instance.poolList[i].mOriginal == go)
                {
                    return instance.poolList[i].Get(activationState);
                }
            }
        }

        Pool newPool = new Pool(go, 5, instance.poolingStrategy);
        instance.poolList.Add(newPool);

        return newPool.Get();
    }
}
