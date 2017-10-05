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

        gamePlayerScript.spellSlot_1.spell = lobbyPlayerScript.spells[0];
        gamePlayerScript.spellSlot_2.spell = lobbyPlayerScript.spells[1];
        gamePlayerScript.spellSlot_3.spell = lobbyPlayerScript.spells[2];
    }
}
