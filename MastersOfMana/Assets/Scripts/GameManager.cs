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
    public A_SpellBehaviour currentlyCastUltiSpell = null;
    public int numOfActiveMenus = 0;
    public bool gameRunning = false;
    public CameraSystem cameraSystem;
    private LavaFloor mLavaFloor;

    public AudioListener listener;

    public delegate void GameStarted();
    public static event GameStarted OnGameStarted;

    public delegate void RoundStarted();
    public static event RoundStarted OnRoundStarted;

    public delegate void LocalPlayerDead();
    public static event LocalPlayerDead OnLocalPlayerDead;

    public delegate void LocalPlayerWon();
    public static event LocalPlayerWon OnLocalPlayerWon;

    public delegate void RoundEnded();
    public static event RoundEnded OnRoundEnded;

    public delegate void HostEndedRound();
    public static event HostEndedRound OnHostEndedRound;

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

            Application.targetFrameRate = 300;
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
        mLavaFloor = FindObjectOfType<LavaFloor>();
        //end ultispells, that are currently running
        if (isUltimateActive)
        {
            isUltimateActive = false;
            mSun.RpcFade(true);
            currentlyCastUltiSpell.EndSpell();
        }
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
            }

            NetManager.instance.RpcTriggerGameStarted();
        }
    }

    public static void ResetEvents()
    {
        OnGameStarted = null;
        OnRoundStarted = null;
        OnRoundEnded = null;
        OnHostEndedRound = null;
        OnLocalPlayerWon = null;
        OnLocalPlayerDead = null;
    }

    public void TriggerGameStarted()
    {
        ResetLocalGameState();
        mStartPoints = FindObjectOfType<StartPoints>();
        if (OnGameStarted != null)
        {
            OnGameStarted();
        }

        //Example for Arne TODO delete this! and the testfunctions!!1!
        mLavaFloor.OnLavaWillRise += ONLYFORTESTINGDELETETHIS;
        mLavaFloor.OnLavaStoppedRising += ONLYFORTESTINGDELETETHIS1;
        var ezs = FindObjectOfType<EnergyZoneSystem>();
        ezs.OnLoadingZoneWillSpawn += ONLYFORTESTINGDELETETHIS2;
        ezs.OnLoadingZoneDisappeared += ONLYFORTESTINGDELETETHIS3;
    }

    // INGAME
    // This is only executed on the Server
    public void PlayerDown() {
        ++mNumberOfDeadPlayers;

        //TODO: What happens if both die simultaniously?
        if (mNumberOfDeadPlayers >= (players.Count - 1)) //only one player left -> he/she won the game!
        { 
            PostGame();
        }
    }

    public void PostGame()
    {
        //Find out who has won and call post game screen
        foreach (var p in players)
        {
            if (p.healthScript.IsAlive())
            {
                winnerID = p.netId.Value;
                break;
            }
        }
        PostGameLobby(winnerID);
        ResetLocalGameState();
    }

    private AnimateLightIntensity mSun;

    public void RegisterUltiSpell(A_SpellBehaviour ultiSpell)
    {
        if(!mSun)
        {
            mSun = GameObject.FindGameObjectWithTag("Sun").GetComponent<AnimateLightIntensity>();
        }

        mSun.RpcFade(false);

        isUltimateActive = true;
        currentlyCastUltiSpell = ultiSpell;
    }

    public void UnregisterUltiSpell(A_SpellBehaviour ultiSpell)
    {
        if (isUltimateActive && currentlyCastUltiSpell == ultiSpell)
        {
            mSun.RpcFade(true);
            isUltimateActive = false;
            currentlyCastUltiSpell = null;
        }
        else
        {
            Debug.LogWarning("Trying to unregister an ultiSpell, that is not registered!");
        }
    }

    public void TriggerRoundEnded()
    {
        //make sure the preview is ended whenever the current round has ended
        localPlayer.GetPlayerSpells().StopPreview();
        localPlayer.inputStateSystem.current.SetPreview(false);

        gameRunning = false;
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
            //If the game is running, check if a menu is currently open
            if(gameRunning)
            {
                Cursor.lockState = numOfActiveMenus > 0 ? CursorLockMode.None : CursorLockMode.Locked;
            }
            //If the game is not running, never lock the mouse
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
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

    public void PlayerDisconnected()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].gameObject.activeSelf)
            {
                players.Remove(players[i]);
                i--;
            }
        }

        //If we don't have a NetManager return!
        if (!NetManager.instance || !NetManager.instance.amIServer())
        {
            return;
        }

            //If the game is runnning we need to check for dead players
            if (gameRunning)
        {
            //We need to recheck if all but one players are dead!
            //Since we can't be sure that we can still access the players Healthscript we need to check the alive status of all remaining players again
            mNumberOfDeadPlayers = 0;
            foreach (PlayerScript player in players)
            {
                if (!player.healthScript.IsAlive())
                {
                    ++mNumberOfDeadPlayers;
                }
            }
            if (mNumberOfDeadPlayers >= (players.Count - 1)) //only one player left -> he/she won the game!
            {
                PostGame();
            }
        }
        //Otherwise we need to check for ready players
        else
        {
            mNumOfReadyPlayers = 0;
            foreach(PlayerScript player in players)
            {
                if (player.playerLobby.isReady)
                {
                    ++mNumOfReadyPlayers;
                }
            }
            if (mNumOfReadyPlayers >= players.Count)
            {
                NetManager.instance.RpcTriggerRoundStarted();
            }
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
			players[i].movement.RpcSetPositionAndRotation(startPositions[i].position, startPositions[i].rotation);
			players[i].movement.RpcSetVelocity(Vector3.zero);
            players[i].RpcSetInputState(InputStateSystem.InputStateID.Normal);
			players[i].enabled = true;
		}
    }

    public void TriggerOnRoundStarted()
    {
        gameRunning = true;
        OnApplicationFocus(true);
        ResetGame();
        listener.enabled = false;
        if (OnRoundStarted != null)
        {
            OnRoundStarted();
        }
	}

    public void TriggerOnLocalPlayerWon()
    {
        if (OnLocalPlayerWon != null)
        {
            OnLocalPlayerWon();
        }
    }

    /// <summary>
    /// Gets a list of all opponents of the given player script.
    /// </summary>
    /// <returns>The opponent.</returns>
    /// <param name="self">Self.</param>
    public List<PlayerScript> GetOpponents(PlayerScript self)
    {
        Debug.Assert(self != null, 
                     "Opponents of null is not very sensical. Either you forgot to set a player or you just want \"GameManager.instance.player\"");

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

    public List<PlayerScript> GetNonLocalPlayers()
    {
        return GetOpponents(localPlayer);
    }

    public float GetLavaFloorHeight()
    {
        return mLavaFloor.transform.position.y;
    }

    public LavaFloor GetLavaFloor()
    {
        return mLavaFloor;
    }

    public void TriggerHostEndedRound()
    {
        if (OnHostEndedRound!=null)
        {
            OnHostEndedRound();
        }
    }

    private void ONLYFORTESTINGDELETETHIS()
    {
        Debug.Log("lava works");
    }

    private void ONLYFORTESTINGDELETETHIS1()
    {
        Debug.Log("lava end works too");
    }

    private void ONLYFORTESTINGDELETETHIS2()
    {
        Debug.Log("zone works");
    }

    private void ONLYFORTESTINGDELETETHIS3()
    {
        Debug.Log("zone end works too");
    }
}
