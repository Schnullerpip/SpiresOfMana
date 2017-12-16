using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public List<PlayerScript> players;

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

    public delegate void RoundEnded();
    public static event RoundEnded OnRoundEnded;

    private int mNumOfReadyPlayers = 0;
    private StartPoints mStartPoints;
    private Player mRewiredPlayer;

    public void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            mRewiredPlayer = ReInput.players.GetPlayer(0);
            instance = this;
            eventSystem.SetActive(true);
            DontDestroyOnLoad(this);
        }
    }

    public void Start()
    {
        players = new List<PlayerScript>();
    }

    public void ResetLocalGameState()
    {
        mNeededToGo = mInitialNeededToGo;
        mNumberOfGoMessages = 0;
        mNumberOfDeadPlayers = 0;
        mNumOfReadyPlayers = 0;
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
        players = allPlayers;
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
            foreach (var p in players)
            {
                p.RpcSetInputState(InputStateSystem.InputStateID.Normal);
                PlayerSpells playerSpells = p.GetPlayerSpells();
                playerSpells.RpcUpdateSpells(playerSpells.spellslot[0].spell.spellID, playerSpells.spellslot[1].spell.spellID, playerSpells.spellslot[2].spell.spellID, playerSpells.spellslot[3].spell.spellID);
            }

            NetManager.instance.RpcTriggerGameStarted();
        }
    }

    public void TriggerGameStarted()
    {
        mStartPoints = FindObjectOfType<StartPoints>();
        if (OnGameStarted != null)
        {
            OnGameStarted();
        }
    }

    // INGAME
    // This is only executed on the Server
    public void PlayerDown() {
        ++mNumberOfDeadPlayers;

        //TODO: What happens if both die simultaniously?
        if (mNumberOfDeadPlayers >= (players.Count - 1)) { //only one player left -> he/she won the game!

            //Find out who has won and call post game screen
            foreach (var p in players)
            {
                if(p.healthScript.IsAlive())
                {
                    winnerID = p.netId.Value;
                    break;
                }
            }
            PostGameLobby(winnerID);
            ResetLocalGameState();
        }
    }

    public void TriggerRoundEnded()
    {
        if (OnRoundEnded != null)
        {
            OnRoundEnded();
        }
    }

    public void PostGameLobby(uint winner) {
        NetManager.instance.RpcRoundEnded(winner);
    }

    public void localPlayerDead()
    {
        localPlayer.SpawnSpectator();
        if(OnLocalPlayerDead != null)
        {
            OnLocalPlayerDead();
        }
    }

    public void SetMenuActive(bool active)
    {
        if(active)
        {
            numOfActiveMenus++;
        }
        else
        {
            numOfActiveMenus--;
        }
        OnApplicationFocus(true);
    }
		
    public void OnApplicationFocus(bool focus)
    {
		if (focus && ReInput.isReady)
        {
            mRewiredPlayer.controllers.maps.SetMapsEnabled(numOfActiveMenus > 0, "UI");
            mRewiredPlayer.controllers.maps.SetMapsEnabled(!(numOfActiveMenus > 0), "Default");
            Cursor.lockState = numOfActiveMenus > 0 ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = numOfActiveMenus > 0;
        }
    }

    //called on the server only!
    public void playerReady()
    {
        mNumOfReadyPlayers++;
        if(mNumOfReadyPlayers >= players.Count)
        {
            NetManager.instance.RpcTriggerRoundStarted();
        }
    }
		
    void ResetGame()
    {
        localPlayer.enabled = true;
		if (!(NetManager.instance.amIServer())) 
		{
			return;
		}
		List<Transform> startPositions = mStartPoints.GetRandomStartPositions();
		for(int i = 0; i < players.Count; i++)
		{
			players[i].movement.RpcSetPosition(startPositions[i].position);
			players[i].movement.RpcSetVelocity(Vector3.zero);
			players[i].enabled = true;
		}
    }

    public void TriggerOnRoundStarted()
    {
        ResetGame();
        if (OnRoundStarted != null)
        {
            OnRoundStarted();
        }
	}

    /// <summary>
    /// Gets a list of all opponents of the given player script.
    /// </summary>
    /// <returns>The opponent.</returns>
    /// <param name="self">Self.</param>
    public List<PlayerScript> GetOpponents(PlayerScript self)
    {
        List<PlayerScript> opponents = new List<PlayerScript>(players.Count - 1);
		for (int i = 0; i < players.Count; ++i)
        {
			if(players[i] != self)
            {
				opponents.Add(players[i]);
            }
        }
        return opponents;
    }
}
