using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PoolRegistry : NetworkBehaviour
{

    public static PoolRegistry instance;
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

    /// <summary>
    /// returns an object out of a pool or creates a pool if it is nonexistent
    /// </summary>
    /// <param name="go"></param>
    /// <param name="poolSize"></param>
    /// <param name="poolGrowth"></param>
    /// <param name="poolingStrategy"></param>
    /// <param name="activationState"></param>
    /// <returns></returns>
    public static GameObject GetInstance(
        GameObject go,
        int poolSize,
        int poolGrowth,
        Pool.PoolingStrategy poolingStrategy = Pool.PoolingStrategy.OnMissSubjoinElements,
        Pool.Activation activationState = Pool.Activation.ReturnDeactivated)
    {
        foreach (Pool p in instance.poolList)
        {
            if (p.mOriginal == go)
            {
                return p.Get(activationState);
            }
        }

        Pool newPool = new Pool(go, poolSize, poolGrowth, poolingStrategy);
        instance.poolList.Add(newPool);

        return newPool.Get(activationState);
    }

    /// <summary>
    /// returns an object out of a pool or creates a pool if it is nonexistent - instances can be given a default transform
    /// </summary>
    /// <param name="go"></param>
    /// <param name="transform"></param>
    /// <param name="poolSize"></param>
    /// <param name="poolGrowth"></param>
    /// <param name="poolingStrategy"></param>
    /// <param name="activationState"></param>
    /// <returns></returns>
    public static GameObject GetInstance(GameObject go, Transform transform, int poolSize, int poolGrowth, Pool.PoolingStrategy poolingStrategy = Pool.PoolingStrategy.OnMissSubjoinElements, Pool.Activation activationState = Pool.Activation.ReturnDeactivated)
    {
        return GetInstance(go, transform.position, transform.rotation, poolSize, poolGrowth, poolingStrategy, activationState);
    }

    /// <summary>
    /// returns an object out of a pool or creates a pool if it is nonexistent - instances can be given a default transform
    /// </summary>
    /// <param name="go"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="poolSize"></param>
    /// <param name="poolGrowth"></param>
    /// <param name="poolingStrategy"></param>
    /// <param name="activationState"></param>
    /// <returns></returns>
    public static GameObject GetInstance(
        GameObject go,
        Vector3 position,
        Quaternion rotation,
        int poolSize,
        int poolGrowth,
        Pool.PoolingStrategy poolingStrategy = Pool.PoolingStrategy.OnMissSubjoinElements,
        Pool.Activation activationState = Pool.Activation.ReturnDeactivated)
    {
        foreach (Pool p in instance.poolList)
        {
            if (p.mOriginal == go)
            {
                return p.Get(position, rotation, activationState);
            }
        }

        Pool newPool = new Pool(go, poolSize, poolGrowth, poolingStrategy, position, rotation);
        instance.poolList.Add(newPool);

        return newPool.Get(position, rotation, activationState);
    }
}