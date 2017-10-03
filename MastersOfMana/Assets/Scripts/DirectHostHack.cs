using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Creates a hosted game on start. Needed to be able to start game directly without Lobby/Menu
/// </summary>
public class DirectHostHack : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GetComponentInParent<NetworkManager>().StartHost();
        GameManager.SetPlayers(new List<PlayerScript>(GameObject.FindObjectsOfType<PlayerScript>()));
    }
}
