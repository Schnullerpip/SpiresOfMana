using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyHook : Prototype.NetworkLobby.LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        PlayerScript gamePlayerScript = gamePlayer.GetComponent<PlayerScript>();
        Prototype.NetworkLobby.LobbyPlayer lobbyPlayerScript = lobbyPlayer.GetComponent<Prototype.NetworkLobby.LobbyPlayer>();

        gamePlayerScript.playerName = lobbyPlayerScript.playerName;
		gamePlayerScript.playerColor = lobbyPlayerScript.playerColor;
        gamePlayerScript.playerColorIndex = System.Array.IndexOf(Prototype.NetworkLobby.LobbyPlayer.Colors, lobbyPlayerScript.playerColor);
    }
}
