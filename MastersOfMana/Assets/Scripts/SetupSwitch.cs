using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupSwitch : MonoBehaviour {

    public GameObject soloGameSetup;
    public GameObject multiplayerSetup;

    public void Awake()
    {
        if (Prototype.NetworkLobby.LobbyManager.s_Singleton)
        {
            Instantiate(multiplayerSetup);
        }
        else
        {
           Instantiate(soloGameSetup, transform.position, transform.rotation);
        }
    }
}
