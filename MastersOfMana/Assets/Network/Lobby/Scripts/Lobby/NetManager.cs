﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetManager : NetworkBehaviour {
    public static NetManager instance;
    public bool initialized = false;
    private HUD mHud;

    private void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        mHud = FindObjectOfType<HUD>();
    }

    public bool amIServer()
    {
        return isServer;
    }

    [ClientRpc]
    public void RpcRoundEnded(uint winner)
    {
        GameManager.instance.winnerID = winner;
        FindObjectOfType<HUD>().ShowPostGameScreen(true);
        GameManager.instance.TriggerRoundEnded();
        if (GameManager.instance.winnerID == GameManager.instance.localPlayer.netId.Value)
        {
            GameManager.instance.TriggerOnLocalPlayerWon();
        }
    }

    [ClientRpc]
    public void RpcTriggerGameStarted()
    {
        GameManager.instance.players.Clear();
        //GameManager on client doe not have the players yet - get them
        var players = FindObjectsOfType<PlayerScript>();
        foreach (var p in players)
        {
            GameManager.instance.players.Add(p);
        }

        GameManager.instance.TriggerGameStarted();
    }

    [ClientRpc]
    public void RpcTriggerRoundStarted()
    {
        GameManager.instance.TriggerOnRoundStarted();
    }

    [ClientRpc]
    public void RpcHostEndedRound()
    {
		if (!isServer) 
		{
        	mHud.ExitPostGameScreen();
		}
		GameManager.instance.TriggerHostEndedRound ();
    }
}
