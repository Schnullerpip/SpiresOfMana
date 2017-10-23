using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SingleMultiSwitchHack : NetworkBehaviour {

	public GameObject[] singleplayerObjects;
	public GameObject[] multiplayerObjects;

	void Start () 
	{
		Debug.LogWarning("This script is for testing purposes only, delete this and place the real  " + multiplayerObjects[0].name + " (and other ointo the scene");

		bool isSolo = Prototype.NetworkLobby.LobbyManager.s_Singleton == null;

		foreach (var go in singleplayerObjects) 
		{
			go.SetActive(isSolo);
		}

		foreach (var go in multiplayerObjects) 
		{
			go.SetActive(!isSolo);
		}

	}
}
