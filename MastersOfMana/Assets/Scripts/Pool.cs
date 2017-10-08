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

    //the original that is copied whenever new elements are instantiated into the pool
    private GameObject mOriginal;

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
    private int mRoundRobinIdx = 0;
    private GameObject OnMissRoundRobin() {
        if (mRoundRobinIdx >= mSize) {
            mRoundRobinIdx = 0;
        }
        return mObjects[mRoundRobinIdx++];
    }
    //----------------------------------the implemented strategys

    //CONSTRUCTOR
    public Pool(GameObject original, int size, PoolingStrategy strategy) {

        mOriginal = original;
        mGrowth = size;

        SubjoinElements();

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

    //The method to call, whenever new elements need to be put into the objects list
    private GameObject SubjoinElements() {
        mSize += mGrowth;
        List<GameObject> newElements = new List<GameObject>();
        for (int i = 0; i < mGrowth; ++i) {
            GameObject newObject;
            newObject = GameObject.Instantiate(mOriginal);
            NetworkServer.Spawn(newObject);
            A_SummoningBehaviour summoning = newObject.GetComponent<A_SummoningBehaviour>();
            if (summoning)
            {
                newObject.SetActive(false);
                summoning.RpcSetActive(false);
            }
            newElements.Add(newObject);
        }

        mObjects.AddRange(newElements);

        return newElements[0];
    }


    public enum Activation { ReturnDeactivated, ReturnActivated};
    /// <summary>
    /// returns an object of the pool according to the used strategy in this pool
    /// if no elements are ready to be returned there are several strategies to use
    /// 1. OnMissReturnNull --> returns null on a miss
    /// 2. OnMissSubjoinElements --> creates new instances and then returns a new one
    /// 3. OnMissRoundRobin --> returns first found inactive element
    /// </summary>
    /// <param name="activateOnReturn"> if true found Instance is activated even before Get returns to caller </param>
    /// <returns></returns>
    public GameObject Get(Activation activateOnReturn = Activation.ReturnDeactivated) {
        GameObject found = null;

        for (int i = 0; i < mSize; ++i)
        {
            found = mObjects[i];
            if (!found.activeSelf)
            {
                break;
            }
            found = null;
        }        

        //Miss! - no active element was found
        if (!found) {
            found =  OnMissBehaviour();
        }

        if (activateOnReturn == Activation.ReturnActivated)
        {
            A_SummoningBehaviour summoning = found.GetComponent<A_SummoningBehaviour>();
            if (summoning)
            {
                Rigidbody rigid = summoning.GetComponent<Rigidbody>();
                if (rigid)
                {
                    rigid.velocity = new Vector3();
                }

                summoning.RpcSetActive(true);
            }
            else
            {
                found.SetActive(true);
            }
        }
        
        return found;
    }
}