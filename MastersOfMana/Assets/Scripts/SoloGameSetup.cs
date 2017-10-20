using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloGameSetup : MonoBehaviour {

    //private GameManager GameManager;
    //private PoolRegistry PoolRegistry;
    //private NetManager NetworkManager;
    public GameObject DirectHostHack;
    public GameObject HealthHUD;
    public GameObject SpellHUD;
    public GameObject IngameMenu;

    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
    }

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        GameObject GameManagerObj = new GameObject("GameManager");
        GameManagerObj.transform.SetParent(this.transform);
        GameManagerObj.AddComponent<GameManager>();
        GameManager.instance.AddPlayerMessageCounter();
        GameManager.instance.AddPlayerMessageCounter();

        GameObject DirectHostHackObject = GameObject.Instantiate(DirectHostHack);
        DirectHostHackObject.transform.SetParent(this.transform);
		DirectHostHack.transform.position = this.transform.position;

        GameObject PoolRegistryObject = new GameObject("PoolRegistry");
        PoolRegistryObject.transform.SetParent(this.transform);
        GameManagerObj.AddComponent<PoolRegistry>();

        GameObject NetworkManagerObject = new GameObject("NetworkManager");
        NetworkManagerObject.transform.SetParent(this.transform);
        NetworkManagerObject.AddComponent<NetManager>();

        GameObject.Instantiate(IngameMenu);

        GameManager.instance.Go();
    }

    public void Init()
    {
        bool spellMissing = false;
        foreach(PlayerScript.SpellSlot slot in GameManager.instance.localPlayer.spellslot)
        {
            if(!slot.spell)
            {
                spellMissing = true;
                break;
            }
        }
        if (spellMissing)
        {
            Debug.Log("Assign spell to player prefab spellslot!");
        }
        else
        {
            GameObject.Instantiate(HealthHUD).GetComponent<HealthHUD>().Init();
            GameObject.Instantiate(SpellHUD).GetComponent<SpellHUD>().Init();
        }
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
    }
}
