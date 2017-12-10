using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLobby : NetworkBehaviour {
    [SyncVar]
    public bool isReady = false;

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
