using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloGameSetup : MonoBehaviour {

	public GameObject gameManagerPrefab;
    public GameObject directHostHack;
    public GameObject healthHUD;
    public GameObject spellHUD;
    public GameObject ingameMenu;
    public GameObject hurtIndicator;


    // Use this for initialization
    void Awake () 
	{
        Cursor.lockState = CursorLockMode.Locked;

		GameObject GameManagerObj = Instantiate(gameManagerPrefab);
        GameManagerObj.transform.SetParent(this.transform);
        GameManager.instance.AddPlayerMessageCounter();
        GameManager.instance.AddPlayerMessageCounter();

		Rewired.ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false,"UI");
		Rewired.ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true,"Default");

        GameObject DirectHostHackObject = GameObject.Instantiate(directHostHack);
        DirectHostHackObject.transform.SetParent(this.transform);
        DirectHostHackObject.transform.position = this.transform.position;

        GameObject PoolRegistryObject = new GameObject("PoolRegistry");
        PoolRegistryObject.transform.SetParent(this.transform);
        GameManagerObj.AddComponent<PoolRegistry>();

        GameObject NetworkManagerObject = new GameObject("NetworkManager");
        NetworkManagerObject.transform.SetParent(this.transform);
        NetworkManagerObject.AddComponent<NetManager>();

        Instantiate(ingameMenu);
        Instantiate(hurtIndicator);

		StartCoroutine(PullingGameStart());
    }

	IEnumerator PullingGameStart()
	{
		//hack: wait for the gamemanager to be done initializing
		while(GameManager.instance.localPlayer == null)
		{
			yield return null;
		}

		GameManager.instance.Go();
		Init();
	}

    public void Init()
    {
        bool spellMissing = false;
        foreach(PlayerSpells.SpellSlot slot in GameManager.instance.localPlayer.GetPlayerSpells().spellslot)
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
            Instantiate(healthHUD);//.GetComponent<HealthHUD>().Init();
            Instantiate(spellHUD);//.GetComponent<SpellHUD>().Init();
        }
        GameManager.instance.localPlayer.transform.position = transform.position;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
    }
}
