using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public List<PlayerScript> mPlayers;

    //private PoolRegistry mPoolRegistry;

    private int mNumberOfGoMessages = 0,
                mNumberOfDeadPlayers = 0;
    private int mInitialNeededToGo = 1; //Need to remember this for resets; Magic Number to allow for GO from poolRegistry and NetManager
    private int mNeededToGo = 1;

    public static GameManager instance;
    public uint winnerID;
    public PlayerScript localPlayer;
    public GameObject eventSystem;
    public bool isUltimateActive = false;
    public int numOfActiveMenus = 0;

    public delegate void GameStarted();
    public static event GameStarted OnGameStarted;

    public delegate void RoundStarted();
    public static event RoundStarted OnRoundStarted;

    public delegate void LocalPlayerDead();
    public static event LocalPlayerDead OnLocalPlayerDead;

    private int mNumOfReadyPlayers = 0;

    public void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            eventSystem.SetActive(true);
            DontDestroyOnLoad(this);
        }
    }

    public void ResetLocalGameState()
    {
        mPlayers = new List<PlayerScript>();
        mNeededToGo = mInitialNeededToGo;
        mNumberOfGoMessages = 0;
        mNumberOfDeadPlayers = 0;
        isUltimateActive = false;
    }

    public void Go()
    {
        mNumberOfGoMessages++;
        if (mNumberOfGoMessages == mNeededToGo)
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
        if (NetManager.instance.amIServer())
        {
            //enable the players to actually do stuff and update the chosen Spells
            foreach (var p in mPlayers)
            {
                p.RpcSetInputState(InputStateSystem.InputStateID.Normal);
                p.RpcUpdateSpells(p.GetPlayerSpells().spellslot[0].spell.spellID, p.GetPlayerSpells().spellslot[1].spell.spellID, p.GetPlayerSpells().spellslot[2].spell.spellID, p.GetPlayerSpells().spellslot[3].spell.spellID);
            }

            NetManager.instance.RpcTriggerGameStarted();
        }
    }

    public void TriggerGameStarted()
    {
        if (OnGameStarted != null)
        {
            OnGameStarted();
        }
    }

    public delegate void GameEnded();
    public event GameEnded OnGameEnded;

    // INGAME
    // This is only executed on the Server
    public void PlayerDown() {
        ++mNumberOfDeadPlayers;

        //TODO: What happens if both die simultaniously?
        if (mNumberOfDeadPlayers >= (mPlayers.Count - 1)) { //only one player left -> he/she won the game!

            //Find out who has won and call post game screen
            foreach (var p in mPlayers)
            {
                if(p.healthScript.IsAlive())
                {
                    winnerID = p.netId.Value;
                    
                    break;
                }
            }
            if (OnGameEnded != null)
            {
                OnGameEnded();
            }
            PostGameLobby(winnerID);
            ResetLocalGameState();
        }
    }

    public void PostGameLobby(uint winner) {
        NetManager.instance.RpcLoadPostGameScreen(winner);
    }

    public void localPlayerDead()
    {
        localPlayer.SpawnSpectator();
        if(OnLocalPlayerDead != null)
        {
            OnLocalPlayerDead();
        }
    }

    public void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = numOfActiveMenus > 0 ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    //called on the server only!
    public void playerReady()
    {
        mNumOfReadyPlayers++;
        if(mNumOfReadyPlayers >= mPlayers.Count)
        {
            if(OnRoundStarted != null)
            {
                OnRoundStarted();
            }
        }
    }

    public void TriggerOnRoundStarted()
    {
        if (OnRoundStarted != null)
        {
            OnRoundStarted();
        }
    }
}
