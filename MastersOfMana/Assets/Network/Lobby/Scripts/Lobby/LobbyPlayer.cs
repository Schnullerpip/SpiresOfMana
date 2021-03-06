using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        public static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();

        public enum ControlSchemes
        {
            SelectAndTrigger = 0,
            SelectEqualsTrigger = 1
        }

        public Button colorButton;
        public InputField nameInput;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;

        public GameObject localIcone;
        public GameObject remoteIcone;
        private Button backButton;
        private bool initialized = false;
        [SyncVar]
        private bool forcedColor = false;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;

        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

		[SyncVar]
		private bool isReady = false;

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }
			removePlayerButton.gameObject.SetActive(LobbyManager.s_Singleton.isHost);
            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
            OnMyColor(playerColor);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

           SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            //b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            nameInput.interactable = false;
            removePlayerButton.interactable = NetworkServer.active;
			Text readyButtonText = readyButton.transform.GetChild (0).GetComponent<Text>();
			//OnClientReady(false);
			if (!isReady) 
			{
				ChangeReadyButtonColor(NotReadyColor);
				readyButtonText.text = "...";
			} else 
			{
				ChangeReadyButtonColor(TransparentColor);
				readyButtonText.text = "READY";
				readyButtonText.color = ReadyColor;
			}
            readyButton.interactable = false;
            colorButton.interactable = false;
        }

        void SetupLocalPlayer()
        {
            nameInput.interactable = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);

            CheckRemoveButton();

            playerColor = PlayerPrefsExtended.GetColor("Playercolor", Color.white);
            CmdSetPlayerColor(playerColor);

            //ChangeReadyButtonColor(JoinColor);

            //readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            //readyButton.interactable = true;
            //EventSystem.current.SetSelectedGameObject(readyButton.gameObject);

            //have to use child count of player prefab already setup as "this.slot" is not set yet
            playerName = PlayerPrefs.GetString("Playername","Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount - 1));
            CmdNameChanged(playerName);

            //we switch from simple name display to name input
            colorButton.interactable = true;
            nameInput.interactable = true;

            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener(OnNameChanged);

            colorButton.onClick.RemoveAllListeners();
            colorButton.onClick.AddListener(OnColorClicked);

            //We need to assign the navigation for down directly, it seems to lose the explicit reference because the back button is on a different UI
            backButton = GetComponentInParent<LobbyPlayerList>().backButton;
            //AssignCustomNavigation(readyButton, backButton, OnSelectDirection.down);
            AssignCustomNavigation(removePlayerButton, backButton, OnSelectDirection.down);
            removePlayerButton.gameObject.SetActive(false);

            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        public enum OnSelectDirection
        {
            down,
            up,
            left,
            right
        }
        public void AssignCustomNavigation(Button sourceButton, Button targetButton, OnSelectDirection direction)
        {
            Navigation navigation = sourceButton.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            switch (direction)
            {
                case OnSelectDirection.up:
                    navigation.selectOnUp = targetButton;
                    break;
                case OnSelectDirection.left:
                    navigation.selectOnLeft = targetButton;
                    break;
                case OnSelectDirection.right:
                    navigation.selectOnRight = targetButton;
                    break;
                default:
                    navigation.selectOnDown = targetButton;
                    break;
            }
            sourceButton.navigation = navigation;
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.interactable = localPlayerCount > 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
				isReady = true;
                //ChangeReadyButtonColor(TransparentColor);

                //Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                //textComponent.text = "READY";
                //textComponent.color = ReadyColor;
                //readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;
                // Push chosen spells to server
                if (isLocalPlayer)
                {
                    PlayerPrefs.SetString("Playername", playerName);
                    if(!forcedColor)
                    {
                        PlayerPrefsExtended.SetColor("Playercolor", playerColor);
                    }
                    PlayerPrefs.Save();
                }
            }
            else
            {
                //ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);
				isReady = false;
                //Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                //textComponent.text = isLocalPlayer ? "JOIN" : "...";
                //textComponent.color = Color.white;
                //readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
            }
        }

        public void OnPlayerListChanged(int idx)
        { 
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
        }

        public void OnMyColor(Color newColor)
        {
            playerColor = newColor;
            colorButton.GetComponent<Image>().color = newColor;
        }

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnColorClicked()
        {
            CmdColorChange();
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        [ClientRpc]
        public void RpcSetPlayerReady()
        {
            if(isLocalPlayer)
            {
                SendReadyToBeginMessage();
            }
        }

        public void OnNameChanged(string str)
        {
            PlayerPrefs.SetString("Playername", str);
            PlayerPrefs.Save();
            CmdNameChanged(str);
        }

        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);
                
        }

        public void ToggleJoinButton(bool enabled)
        {
            //readyButton.gameObject.SetActive(enabled);
            //waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.mainMenu.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.mainMenu.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcGameStarts()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        [Command]
        public void CmdSetPlayerColor(Color color)
        {
            playerColor = color;
            CmdColorChange();
            initialized = true;
            if(playerColor != color)
            {
                forcedColor = true;
            }
        }

        [Command]
        public void CmdColorChange()
        {
            forcedColor = false;
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            //idx = (idx + 1) % Colors.Length;

            bool alreadyInUse = false;

            do
            {
                alreadyInUse = false;
                for (int i = 0; i < _colorInUse.Count; ++i)
                {
                    if (_colorInUse[i] == idx)
                    {//that color is already in use
                        alreadyInUse = true;
                        idx = (idx + 1) % Colors.Length;
                    }
                }
            }
            while (alreadyInUse);

            if (inUseIdx >= 0 && initialized)//If its not initialized yet, make sure  we add the color and don't overwrite another one!
            {//if we already add an entry in the colorTabs, we change it
                _colorInUse[inUseIdx] = idx;
            }
            else
            {//else we add it
                _colorInUse.Add(idx);
            }

            playerColor = Colors[idx];
        }

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx < 0)
                return;

            for (int i = 0; i < _colorInUse.Count; ++i)
            {
                if (_colorInUse[i] == idx)
                {//that color is already in use
                    _colorInUse.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
