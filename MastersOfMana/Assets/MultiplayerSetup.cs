using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSetup : MonoBehaviour {

    public List<GameObject> ObjectsOnGamestart;

    public void OnEnable()
    {
        foreach(GameObject obj in ObjectsOnGamestart)
        {
            GameObject.Instantiate(obj);
        }
    }
}
