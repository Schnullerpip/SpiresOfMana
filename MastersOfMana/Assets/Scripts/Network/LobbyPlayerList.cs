using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    //List of players in the lobby
    public class LobbyPlayerList : MonoBehaviour
    {
        public static LobbyPlayerList _instance = null;

        public RectTransform playerListContentTransform;
        public GameObject warningDirectPlayServer;
        public Button backButton;
        public Button startButton;
        public Text waitingText;
        public Text countdownText;
        public RectTransform panel;
        public CanvasGroup canvasGroup;

        protected VerticalLayoutGroup _layout;
        protected List<LobbyPlayer> _players = new List<LobbyPlayer>();

        private void Start()
        {
            backButton.onClick.AddListener(GoBack);
        }

        public void OnEnable()
        {
            canvasGroup.interactable = true;
            //Only the Server can Start the game!
            startButton.gameObject.SetActive(LobbyManager.s_Singleton.isHost);
            startButton.interactable = true;
            waitingText.gameObject.SetActive(!LobbyManager.s_Singleton.isHost);
            LobbyManager.s_Singleton.SetCancelDelegate(GoBack);
            _instance = this;
            _layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();
        }


        public void OnDisable()
        {
            LobbyManager.s_Singleton.RemoveLastCancelDelegate();
        }

        public void OnStartClicked()
        {
            startButton.interactable = false;
            foreach (LobbyPlayer player in _players)
            {
                player.RpcSetPlayerReady();
            }
        }

        void Update()
        {
            //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
            //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
            
            if(_layout)
                _layout.childAlignment = Time.frameCount%2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
        }

        public void AddPlayer(LobbyPlayer player)
        {
            if (_players.Contains(player))
                return;

            _players.Add(player);

            player.transform.SetParent(playerListContentTransform, false);

            PlayerListModified();
        }

        public void GoBack()
        {
            LobbyManager.s_Singleton.StopHostClbk();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            _players.Remove(player);
            PlayerListModified();
        }

        public void PlayerListModified()
        {
            int i = 0;
            foreach (LobbyPlayer p in _players)
            {
                p.OnPlayerListChanged(i);
                ++i;
            }
        }
    }
}
