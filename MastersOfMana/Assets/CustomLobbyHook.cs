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

        gamePlayerScript.UpdateSpells(lobbyPlayerScript.spells[0].spellID, lobbyPlayerScript.spells[1].spellID, lobbyPlayerScript.spells[2].spellID);
    }
}
