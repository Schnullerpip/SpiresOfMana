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
    private static int mNeededToGo = 1; //Magic Number to allow for GO from poolRegistry

    public static void ResetLocalGameState()
    {
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
        }
    }

    // INGAME
    public static void PlayerDown() {
        ++mNumberOfDeadPlayers;

        if (mNumberOfDeadPlayers > (mPlayers.Count - 1)) { //only one player left -> he/she won the game!
            Debug.Log("Last Mage standing");
            ResetLocalGameState();
        }

    }

    IEnumerator PostGameLobby(PlayerScript winner, List<PlayerScript> others) {
        yield return new WaitForSeconds(5.0f);
        //TODO change to postGameLobbyScene
        //SceneManager.LoadSceneAsync(indexOfPostGameLobbyScene);
    }

    public void Update()
    {
    }
}
