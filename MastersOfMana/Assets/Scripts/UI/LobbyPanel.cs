using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : MonoBehaviour {

	public List<PlayerStatus> playerStatus;

	public void OnEnable()
	{
		List<PlayerScript> players = GameManager.instance.players;
		for (int i = 0; i < playerStatus.Count; i++) 
		{
            bool playerExists = i < players.Count;
            playerStatus[i].gameObject.SetActive(playerExists);
            if (playerExists)
            {
                playerStatus[i].SetName(players[i].playerName);
                playerStatus[i].SetStatus(players[i].playerLobby.isReady);
            }
		}
	}

	public void Update()
	{
		List<PlayerScript> players = GameManager.instance.players;
		for (int i = 0; i < players.Count; i++) 
		{
			playerStatus[i].SetStatus(players[i].playerLobby.isReady);
		}
	}
}
