using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSetup : MonoBehaviour {

    public List<GameObject> ObjectsOnGamestart;

    void Awake()
    {
        foreach (GameObject obj in ObjectsOnGamestart)
        {
		    GameObject.Instantiate(obj);
        }
    }
}
