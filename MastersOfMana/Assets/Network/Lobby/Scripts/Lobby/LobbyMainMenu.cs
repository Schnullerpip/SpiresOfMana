using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;



        [Header("UI Reference")]

        public RectTransform mainMenuPanel;
        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;
        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public RectTransform spellSelectionPanel;
        public RectTransform optionsPanel;
        public InputField playernameField;

        public InputField matchNameInput;
        public Button backButton;

		private Rewired.Player rewiredPlayer;

		public Rewired.Player GetRewiredPlayer()
		{
			return rewiredPlayer;
		}

        public void OnEnable()
        {
            matchNameInput.onEndEdit.RemoveAllListeners();
            matchNameInput.onEndEdit.AddListener(onEndEditGameName);

            lobbyManager = GameObject.FindObjectOfType<LobbyManager>();
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(lobbyManager.GoBackButton);

			rewiredPlayer = ReInput.players.GetPlayer (0);
            string playername = PlayerPrefs.GetString("Playername", "DefaultName");
            matchNameInput.text = playername + "'s Gameroom";
            playernameField.text = playername;
        }

        public void OnClickHost()
        {
            lobbyManager.StartHost();
        }

        public void OnClickJoin()
        {
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.StartClient();

            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
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
            lobbyManager.StartMatchMaker();
            lobbyManager.SetCancelDelegate(lobbyManager.SimpleBackClbk);
            lobbyManager.backDelegate = lobbyManager.Cancel;
            backButton.gameObject.SetActive(true);
            lobbyManager.ChangeTo(lobbyServerList);
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
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
            PlayerPrefs.SetString("Playername", input.text);
            PlayerPrefs.Save();
            matchNameInput.text = input.text + "'s Gameroom";
        }
    }
}
