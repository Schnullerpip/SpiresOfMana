using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Rewired;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;
		public CustomNetworkDiscovery networkDiscovery;

        [Header("UI Reference")]

        public RectTransform mainMenuPanel;
        public RectTransform lobbyServerList;
        public LobbyPlayerList lobbyPanel;
        public LobbyInfoPanel infoPanel;
        public RectTransform spellSelectionPanel;
        public RectTransform optionsPanel;
        public InputField playernameField;
        private int playerColorIndex = 0;
        public Image playerColor;
        public LoadingScreen loadingScreen;

        public InputField matchNameInput;
		public InputField ipInput;
        public Button backButton;

		private Rewired.Player rewiredPlayer;

		public Rewired.Player GetRewiredPlayer()
		{
			return rewiredPlayer;
		}

        private void Awake()
        {
            optionsPanel.GetComponent<LobbyOptions>().ApplySettings();
        }

        public void OnEnable()
        {
            if (GameManager.instance)
            {
                GameManager.instance.listener.enabled = true;
            }
            if(lobbyManager)
            {
                lobbyManager.sceneChangeAllowed = false;
            }
            matchNameInput.onEndEdit.RemoveAllListeners();
            matchNameInput.onEndEdit.AddListener(onEndEditGameName);

			ipInput.onEndEdit.RemoveAllListeners();
			ipInput.onEndEdit.AddListener(onEndEditIP);

			rewiredPlayer = ReInput.players.GetPlayer (0);
            string playername = PlayerPrefs.GetString("Playername", "DefaultName");
            matchNameInput.text = playername + "'s Gameroom";
            playernameField.text = playername;
            //playerColor.color = PlayerPrefsExtended.GetColor("Playercolor", LobbyPlayer.Colors[playerColorIndex]);
            //playerColorIndex = System.Array.IndexOf(LobbyPlayer.Colors, playerColor.color);
        }

        public void OnClickNextPlayerColor()
        {
            playerColorIndex++;
            if (playerColorIndex > LobbyPlayer.Colors.Length - 1)
            {
                playerColorIndex = 0;
            }
            playerColor.color = LobbyPlayer.Colors[playerColorIndex];
        }

        public void OnClickPreviousPlayerColor()
        {
            playerColorIndex--;
            if(playerColorIndex < 0)
            {
                playerColorIndex = LobbyPlayer.Colors.Length - 1;
            }
            playerColor.color = LobbyPlayer.Colors[playerColorIndex];
        }

        public void OnClickHost()
        {
            lobbyManager.isHost = true;
            lobbyManager.isLocalGame = true;
            lobbyManager.StartHost();
            
            networkDiscovery.CustomStartAsServer();
            lobbyPanel.localIpText.gameObject.SetActive(LobbyManager.s_Singleton.networkDiscovery.isServer);
            if (LobbyManager.s_Singleton.networkDiscovery.isServer)
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        lobbyPanel.localIpText.text = "IP: " + ip.ToString();
                    }
                }
            }
        }

        public void OnClickDedicated()
        {
            lobbyManager.ChangeTo(null);
            lobbyManager.StartServer();

            lobbyManager.backDelegate = lobbyManager.StopServerClbk;

            lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
        }

        public void OnClickCreateMatchmakingGame()
        {
            lobbyManager.isHost = true;
            lobbyManager.isLocalGame = false;
            lobbyManager.StartMatchMaker();
            lobbyManager.matchMaker.CreateMatch(
                matchNameInput.text,
                (uint)lobbyManager.maxPlayers,
                true,
				"", "", "", 0, 0,
				lobbyManager.OnMatchCreate);

            //lobbyManager.backDelegate = lobbyManager.StopHost;
            lobbyManager.mIsMatchmaking = true;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
        }

        public void OnClickOpenServerList()
        {
            lobbyManager.isHost = false;
            lobbyManager.StartMatchMaker();
            lobbyManager.SetCancelDelegate(lobbyManager.SimpleBackClbk);
            lobbyManager.backDelegate = lobbyManager.Cancel;
            backButton.gameObject.SetActive(true);
            lobbyManager.ChangeTo(lobbyServerList);
			networkDiscovery.Initialize ();
            networkDiscovery.CustomStartAsClient();
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //OnClickJoin();
            }
        }

        void onEndEditGameName(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }

        public void OnClickOptions()
        {
            lobbyManager.ChangeTo(optionsPanel);
			lobbyManager.SetCancelDelegate (lobbyManager.SimpleBackClbk);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
        }

        public void OnPlayerNameChanged(InputField input)
        {
            if(input.text == "")
            {
                input.text = PlayerPrefs.GetString("Playername", "PlayerName");
            }
            PlayerPrefs.SetString("Playername", input.text);
            PlayerPrefs.Save();
            matchNameInput.text = input.text + "'s Gameroom";
        }
    }
}
