using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLobby : NetworkBehaviour {
    [SyncVar]
    public bool isReady = false;

	public void OnEnable()
	{
		GameManager.OnRoundEnded += RoundEnded;
	}

	public void OnDisable()
	{
		GameManager.OnRoundEnded -= RoundEnded;
	}

	void RoundEnded()
	{
		isReady = false;
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
