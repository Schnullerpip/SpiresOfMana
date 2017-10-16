using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Pool, for instances that handles Object insantiation
/// if no elements are ready to be returned there are several strategies to use
/// 1. OnMissReturnNull --> returns null on a miss
/// 2. OnMissSubjoinElements --> creates new instances and then returns a new one
/// 3. OnMissRoundRobin --> returns first found inactive element
/// </summary>
public class Pool {

    //important members 
    private int mRoundRobinIdx = 0;

    //the original that is copied whenever new elements are instantiated into the pool
    private readonly GameObject mOriginal;
    private NetworkHash128 assetID;

    //the list with the actual objects
    private List<GameObject> mObjects = new List<GameObject>();

    //the initial size of the pool, and also the 'growth' by which the pool grows in case the strategy demands it
    private int mSize = 0;
    private int mGrowth = 0;

    public enum PoolingStrategy { OnMissSubjoinElements, OnMissReturnNull, OnMissRoundRobin };

    //delegate to the used pooling strategy
    private delegate GameObject mStrategy();
    private mStrategy OnMissBehaviour;

    //----------------------------------the implemented strategys
    private GameObject OnMissSubjoinElements() {
        GameObject found = SubjoinElements();
        return found;
    }
    private GameObject OnMissReturnNull() {
        return null;
    }
    private GameObject OnMissRoundRobin() {
        return mObjects[mRoundRobinIdx];
    }
    //----------------------------------the implemented strategys

    //CONSTRUCTOR
    public Pool(GameObject original, int size, PoolingStrategy strategy) {

        mOriginal = original;
        mGrowth = size;

        //get the originals assetID
        assetID = original.GetComponent<NetworkIdentity>().assetId;

        //create some elements
        SubjoinElements();

        ClientScene.RegisterSpawnHandler(assetID, SpawnObject, UnspawnObject);

        //define the pools strategy
        switch (strategy) {
            case PoolingStrategy.OnMissReturnNull:
                OnMissBehaviour = OnMissReturnNull;
                break;
            case PoolingStrategy.OnMissSubjoinElements:
                OnMissBehaviour = OnMissSubjoinElements;
                break;
            case PoolingStrategy.OnMissRoundRobin:
                OnMissBehaviour = OnMissRoundRobin;
                break;
            default:
                OnMissBehaviour = OnMissReturnNull;
                break;
        }
    }

    //The method to call, whenever new elements need to be put into the mObjects list
    private GameObject SubjoinElements() {
        mRoundRobinIdx = mSize;
        mSize += mGrowth;
        List<GameObject> newElements = new List<GameObject>();
        for (int i = 0; i < mGrowth; ++i) {
            //create a new Instance of the original
            GameObject newObject;
            //put the new instance to somewhere far far away, e.g. to reduce network interpolation problems
            newObject = GameObject.Instantiate(mOriginal, A_SummoningBehaviour.OBLIVION, Quaternion.identity);

            //create the instance on the clients
            //NetworkServer.Spawn(newObject); this will now happen ingame... lame i know...


            //deactivate the poolinstance per default
            newObject.SetActive(false);

            //add the new element to the new list
            newElements.Add(newObject);
        }

        //add the new elements to elements
        mObjects.AddRange(newElements);

        return newElements[0];
    }


    public enum Activation { ReturnDeactivated, ReturnActivated};
    /// <summary>
    /// returns an object of the pool according to the used strategy in this pool
    /// this does    NOT   spawn the object on the clients! if you want to do this you need to manually call NetworkServer.Spawn(pool.Get())
    /// if no elements are ready to be returned there are several strategies to use
    /// 1. OnMissReturnNull --> returns null on a miss
    /// 2. OnMissSubjoinElements --> creates new instances and then returns a new one
    /// 3. OnMissRoundRobin --> returns first found inactive element
    /// </summary>
    /// <param name="activateOnReturn"> if true found Instance is activated even before Get returns to caller </param>
    /// <returns></returns>
    public GameObject Get(Activation activateOnReturn = Activation.ReturnDeactivated) {
        GameObject found = null;

        //point to the next object
        if ((++mRoundRobinIdx) >= mSize)
        {
            mRoundRobinIdx = 0;
        }

        //if the current index poitns to an inactive instance take it, else invoke the OnMissBehaviour
        found = !mObjects[mRoundRobinIdx].activeSelf ? mObjects[mRoundRobinIdx] : OnMissBehaviour();

        if (found && (activateOnReturn == Activation.ReturnActivated))
        {
            found.SetActive(true);
        }
        
        return found;
    }

    /// <summary>
    /// handles spawning
    /// </summary>
    /// <param name="position"></param>
    /// <param name="assetID"></param>
    /// <returns></returns>
    public GameObject SpawnObject(Vector3 position, NetworkHash128 assetID)
    {
        //return Get();
        return null;
    }
    /// <summary>
    /// handles despawning
    /// </summary>
    /// <param name="spawned"></param>
    public void UnspawnObject(GameObject spawned)
    {
        spawned.SetActive(false);
    }
}