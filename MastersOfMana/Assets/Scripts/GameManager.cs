using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private List<PlayerScript> mPlayers;

    private PoolRegistry mPoolRegistry;

    private int mNumberOfGoMessages = 0,
                mNumberOfDeadPlayers = 0;
    private int mInitialNeededToGo = 1; //Need to remember this for resets; Magic Number to allow for GO from poolRegistry and NetManager
    private int mNeededToGo = 1;

    public static GameManager instance;
    public string winnerName;
    public PlayerScript localPlayer;

    public delegate void GameStarted();
    public static event GameStarted OnGameStarted;

    public void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void ResetLocalGameState()
    {
        mNeededToGo = mInitialNeededToGo;
        mNumberOfGoMessages = 0;
        mNumberOfDeadPlayers = 0;
    }

    public void Go()
    {
        mNumberOfGoMessages++;
        if(mNumberOfGoMessages == mNeededToGo)
        {
            StartGame();
        }
    }

    public void SetPlayers(List<PlayerScript> allPlayers)
    {
        mPlayers = allPlayers;
    }

    public void AddPlayerMessageCounter()
    {

        mNeededToGo++;
    }

    public void StartGame()
    {
        //activate the pools, to start isntantiating, now that all the players have joined the game
        mPoolRegistry = GameObject.FindObjectOfType<PoolRegistry>();
        mPoolRegistry.CreatePools();

        //enable the players to actually do stuff
        foreach (var p in mPlayers)
        {
            p.RpcChangeInputState(InputStateSystem.InputStateID.Normal);
        }

        //Let everyone know who chose which spells
        localPlayer.RpcShareSpellselection();
        if (GameManager.OnGameStarted != null)
        {
            GameManager.OnGameStarted();
        }
        NetManager.instance.RpcTriggerGameStarted();
    }

    public void TriggerGameStarted()
    {
        if (GameManager.OnGameStarted != null)
        {
            GameManager.OnGameStarted();
        }
    }

    // INGAME
    // This is only executed on the Server
    public void PlayerDown() {
        ++mNumberOfDeadPlayers;

        if (mNumberOfDeadPlayers > (mPlayers.Count - 1)) { //only one player left -> he/she won the game!
            Debug.Log("Last Mage standing");
            ResetLocalGameState();

            //Find out who has won and call post game screen
            foreach (var p in mPlayers)
            {
                if(p.healthScript.IsAlive())
                {
                    winnerName = p.playerName;
                    break;
                }
            }
            StartCoroutine(PostGameLobby(winnerName));
        }
    }

    public IEnumerator PostGameLobby(string winner) {
        yield return new WaitForSeconds(3.0f);
        NetManager.instance.RpcLoadPostGameScreen(winner);
    }
}
