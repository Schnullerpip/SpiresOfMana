using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerSetup : NetworkBehaviour {

    public List<GameObject> ObjectsOnGamestart;

    void Awake()
    {
        foreach (GameObject obj in ObjectsOnGamestart)
        {
            //GameObject spawnedObj = 
				GameObject.Instantiate(obj);
        }
    }
}
