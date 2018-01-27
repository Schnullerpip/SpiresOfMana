using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Creates a hosted game on start. Needed to be able to start game directly without Lobby/Menu
/// </summary>
public class DirectHostHack : MonoBehaviour {

	// Use this for initialization
	public void Start ()
    {
        GetComponentInParent<NetworkManager>().StartHost();
    }
}
