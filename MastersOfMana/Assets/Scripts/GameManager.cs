using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{

    private List<PlayerScript> mPlayers;

    private PoolRegistry mPoolRegistry;

	// Use this for initialization
	void Start ()
	{
	   mPoolRegistry = FindObjectOfType<PoolRegistry>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame(List<PlayerScript> allPlayers)
    {
        mPlayers = allPlayers;

        //activate the pools, to start isntantiating, now that all the players have joined the game
        mPoolRegistry.CreatePools();

        //enable the players to actually do stuff
        foreach (var p in mPlayers)
        {
            p.inputStateSystem.current = p.inputStateSystem.GetState(InputStateSystem.InputStateID.Normal);
        }
    }
}
