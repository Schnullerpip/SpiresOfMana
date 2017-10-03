using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{

    private static List<PlayerScript> mPlayers;

    private static PoolRegistry mPoolRegistry;

    private static int mNumberOfGoMessages = 0;
    private static int mNeededToGo = 1; //Magic Number to allow for GO from poolRegistry

    public static void ResetMessages()
    {
        mNumberOfGoMessages = 0;
    }

    public static void Go()
    {
        mNumberOfGoMessages++;
        if(mNumberOfGoMessages == mNeededToGo)
        {
            StartGame();
        }
    }

    public static void SetPlayers(List<PlayerScript> allPlayers)
    {
        mPlayers = allPlayers;
    }

    public static void AddPlayerMessageCounter()
    {
        mNeededToGo++;
    }

    public static void StartGame()
    {
	    mPoolRegistry = FindObjectOfType<PoolRegistry>();
        //activate the pools, to start isntantiating, now that all the players have joined the game
        mPoolRegistry.CreatePools();

        //enable the players to actually do stuff
        foreach (var p in mPlayers)
        {
            p.RpcChangeInputState(InputStateSystem.InputStateID.Normal);
        }
    }
}
