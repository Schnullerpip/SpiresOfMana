using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLobby : NetworkBehaviour {
    [SyncVar]
    public bool isReady = false;

	public void Awake()
	{
		GameManager.OnRoundEnded += RoundEnded;
	}

	public void OnDestroy()
	{
		GameManager.OnRoundEnded -= RoundEnded;
	}

	void RoundEnded()
	{
        if (isLocalPlayer)
        {
            CmdSetReady(false);
        }
        else
        {
            //This is just so we don't have "Ready" displayed in the lobby for all players that have not ended their round yet
            isReady = false;
        }
	}

    [Command]
    public void CmdSetReady(bool ready)
    {
        isReady = ready;
        if(ready)
        {
            GameManager.instance.playerReady();
        }
    }
}
