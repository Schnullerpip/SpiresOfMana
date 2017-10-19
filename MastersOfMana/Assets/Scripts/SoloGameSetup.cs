using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloGameSetup : MonoBehaviour {

    private GameManager GameManager;
    private PoolRegistry PoolRegistry;
    private NetManager NetworkManager;
    public GameObject DirectHostHack;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        GameObject GameManagerObj = new GameObject("GameManager");
        GameManagerObj.transform.SetParent(this.transform);
        GameManager = GameManagerObj.AddComponent<GameManager>();
        GameManager.instance.AddPlayerMessageCounter();

        GameObject PoolRegistryObject = new GameObject("PoolRegistry");
        PoolRegistryObject.transform.SetParent(this.transform);
        PoolRegistry = GameManagerObj.AddComponent<PoolRegistry>();

        GameObject NetworkManagerObject = new GameObject("NetworkManager");
        NetworkManagerObject.transform.SetParent(this.transform);
        NetworkManager = NetworkManagerObject.AddComponent<NetManager>();

        GameObject DirectHostHackObject = new GameObject("DirectHostHack");
        DirectHostHack = GameObject.Instantiate(DirectHostHack);
        DirectHostHack.transform.SetParent(this.transform);
    }
}
