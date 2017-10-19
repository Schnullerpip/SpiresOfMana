using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloGameSetup : MonoBehaviour {

    //private GameManager GameManager;
    //private PoolRegistry PoolRegistry;
    //private NetManager NetworkManager;
    public GameObject DirectHostHack;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        GameObject GameManagerObj = new GameObject("GameManager");
        GameManagerObj.transform.SetParent(this.transform);
        GameManagerObj.AddComponent<GameManager>();
        GameManager.instance.AddPlayerMessageCounter();

        GameObject PoolRegistryObject = new GameObject("PoolRegistry");
        PoolRegistryObject.transform.SetParent(this.transform);
        GameManagerObj.AddComponent<PoolRegistry>();

        GameObject NetworkManagerObject = new GameObject("NetworkManager");
        NetworkManagerObject.transform.SetParent(this.transform);
        NetworkManagerObject.AddComponent<NetManager>();

        GameObject DirectHostHackObject = GameObject.Instantiate(DirectHostHack);
        DirectHostHackObject.transform.SetParent(this.transform);
    }
}
