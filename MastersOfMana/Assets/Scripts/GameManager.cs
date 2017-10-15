using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{

    private static List<PlayerScript> mPlayers;
    private static List<PlayerHealthScript> mPlayerHealths;

    private static PoolRegistry mPoolRegistry;

    private static int mNumberOfGoMessages = 0,
                       mNumberOfDeadPlayers = 0;
    private static int mInitialNeededToGo = 1; //Need to remember this for resets; Magic Number to allow for GO from poolRegistry
    private static int mNeededToGo = mInitialNeededToGo; 

    public static void ResetLocalGameState()
    {
        mNeededToGo = mInitialNeededToGo;
        mNumberOfGoMessages = 0;
        mNumberOfDeadPlayers = 0;
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
            if (p.isLocalPlayer)
            {
                p.RpcShareSpellselection();
            }
        }
    }

    // INGAME
    public static void PlayerDown() {
        ++mNumberOfDeadPlayers;

        if (mNumberOfDeadPlayers > (mPlayers.Count - 1)) { //only one player left -> he/she won the game!
            ResetLocalGameState();
            //TODO: Who's still alive, who won?
            PostGameLobby(mPlayers[0], mPlayers);
        }
    }

    public static IEnumerator PostGameLobby(PlayerScript winner, List<PlayerScript> others) {
        yield return new WaitForSeconds(3.0f);
        //TODO show correct winner in UI
        SceneManager.LoadSceneAsync(2,LoadSceneMode.Additive);
    }
}
