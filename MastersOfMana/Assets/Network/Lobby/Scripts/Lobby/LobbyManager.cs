using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;


namespace Prototype.NetworkLobby
{
    public class LobbyManager : NetworkLobbyManager 
    {
        static short MsgKicked = MsgType.Highest + 1;

        static public LobbyManager s_Singleton;
        public GameObject netManager;

        [Header("Unity UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        protected RectTransform currentPanel;

        public LobbyMainMenu mainMenu;

        public bool isInGame = false;

        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int playerNumber = 0;

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool mIsMatchmaking = false;

        protected bool mDisconnectServer = false;
        
        protected ulong mCurrentMatchID;

        protected LobbyHook mLobbyHooks;
        protected List<LobbyPlayer> mPlayers = new List<LobbyPlayer>();
        protected List<PlayerScript> mLoadedPlayers = new List<PlayerScript>();


        void Start()
        {
            s_Singleton = this;
            mLobbyHooks = GetComponent<Prototype.NetworkLobby.LobbyHook>();
            currentPanel = mainMenu.mainMenuPanel;

            mainMenu.backButton.gameObject.SetActive(false);

            DontDestroyOnLoad(gameObject);
            SetServerInfo("Offline", "None");
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {

            if (SceneManager.GetSceneAt(0).name == lobbyScene)
            {
                if (isInGame)
                {
                    ChangeTo(mainMenu.lobbyPanel);
                    if (mIsMatchmaking)
                    {
                        if (conn.playerControllers[0].unetView.isServer)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                    else
                    {
                        if (conn.playerControllers[0].unetView.isClient)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                }
                else
                {
                    ChangeTo(mainMenu.mainMenuPanel);
                }

                isInGame = false;
            }
            else
            {
                ChangeTo(null);

                Destroy(GameObject.Find("MainMenuUI(Clone)"));

                //backDelegate = StopGameClbk;
                isInGame = true;
            }
        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != mainMenu.mainMenuPanel && newPanel != null)
            {
                mainMenu.backButton.gameObject.SetActive(true);
            }
            else
            {
                mainMenu.backButton.gameObject.SetActive(false);
                //SetServerInfo("Offline", "None");
                mIsMatchmaking = false;
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;

            mainMenu.infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            //statusInfo.text = status;
            //hostInfo.text = host;
        }

        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            backDelegate();
            isInGame = false;
        }

        // ----------------- Server management

        public void AddLocalPlayer()
        {
            TryToAddPlayer();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(mainMenu.mainMenuPanel);
        }
                 
        public void StopHostClbk()
        {
            if (mIsMatchmaking)
            {
				matchMaker.DestroyMatch((NetworkID)mCurrentMatchID, 0, OnDestroyMatch);
				mDisconnectServer = true;
            }
            else
            {
                StopHost();
            }

            
            ChangeTo(mainMenu.mainMenuPanel);
        }

        public void StopClientClbk()
        {
            StopClient();

            if (mIsMatchmaking)
            {
                StopMatchMaker();
            }

            ChangeTo(mainMenu.mainMenuPanel);
        }

        public void StopServerClbk()
        {
            StopServer();
            ChangeTo(mainMenu.mainMenuPanel);
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }
        
        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            mainMenu.infoPanel.Display("Kicked by Server", "Close", null);
            netMsg.conn.Disconnect();
        }

        //===================

        public override void OnStartHost()
        {
            base.OnStartHost();
            GameManager.instance.ResetLocalGameState();
            ChangeTo(mainMenu.lobbyPanel);
            backDelegate = StopHostClbk;
            SetServerInfo("Hosting", networkAddress);
        }

		public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			base.OnMatchCreate(success, extendedInfo, matchInfo);
            mCurrentMatchID = (System.UInt64)matchInfo.networkId;
		}

		public override void OnDestroyMatch(bool success, string extendedInfo)
		{
			base.OnDestroyMatch(success, extendedInfo);
			if (mDisconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            playerNumber += count;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            //mainMenu.addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
        }

        // ----------------- Server callbacks ------------------

        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);


            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }

            return obj;
        }

        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers >= minPlayers);
                }
            }

        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //Keep track of players that finished loading
            mLoadedPlayers.Add(gamePlayer.GetComponent<PlayerScript>());

            //Tell gamemanager, everyones has finished loading
            if(mLoadedPlayers.Count == numPlayers)
            {
                GameManager.instance.SetPlayers(mLoadedPlayers);
            }

            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.
            if (mLobbyHooks)
                mLobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            return true;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {
			bool allready = true;
			for(int i = 0; i < lobbySlots.Length; ++i)
			{
				if(lobbySlots[i] != null)
					allready &= lobbySlots[i].readyToBegin;
			}

            mLoadedPlayers.Clear();

			if(allready)
				StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                    GameManager.instance.AddPlayerMessageCounter();
                }
            }
			//lock the mouse
			Cursor.lockState = CursorLockMode.Locked;

			Rewired.Player rewiredPlayer = Rewired.ReInput.players.GetPlayer(0);
			            
            ServerChangeScene(playScene);
        }

        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            mainMenu.infoPanel.gameObject.SetActive(false);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                ChangeTo(mainMenu.lobbyPanel);
                backDelegate = StopClientClbk;
                SetServerInfo("Client", networkAddress);
            }
        }
        
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(mainMenu.mainMenuPanel);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(mainMenu.mainMenuPanel);
            mainMenu.infoPanel.Display("Client error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }
    }
}
