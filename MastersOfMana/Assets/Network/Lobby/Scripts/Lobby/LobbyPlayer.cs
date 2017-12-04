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
        static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
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

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;

        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f / 255.0f, 0.0f, 101.0f / 255.0f, 1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        public Button
            spellButton1,
            spellButton2,
            spellButton3,
            spellButton4;

        //spellslots
        public List<A_Spell> spells;
        private SpellRegistry spellregistry;

        private int mSpellToChange;

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

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

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            nameInput.interactable = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);

            CheckRemoveButton();

            if (playerColor == Color.white)
                CmdColorChange();

            ChangeReadyButtonColor(JoinColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            readyButton.interactable = true;
            EventSystem.current.SetSelectedGameObject(readyButton.gameObject);

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

            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);

            spellregistry = Prototype.NetworkLobby.LobbyManager.s_Singleton.mainMenu.spellSelectionPanel.GetComponent<SpellSelectionPanel>().spellregistry;
            spells[0] = spellregistry.GetSpellByID(PlayerPrefs.GetInt("SpellSlot0",0));
            spells[1] = spellregistry.GetSpellByID(PlayerPrefs.GetInt("SpellSlot1",1));
            spells[2] = spellregistry.GetSpellByID(PlayerPrefs.GetInt("SpellSlot2",2));
            spells[3] = spellregistry.GetSpellByID(PlayerPrefs.GetInt("SpellSlot3", 11));

            spellButton1.gameObject.SetActive(true);
            spellButton2.gameObject.SetActive(true);
            spellButton3.gameObject.SetActive(true);
            spellButton4.gameObject.SetActive(true);
            spellButton1.interactable = true;
            spellButton2.interactable = true;
            spellButton3.interactable = true;
            spellButton4.interactable = true;
            ValidateSpellSelection();

            //We need to assign the navigation for down directly, it seems to lose the explicit reference because the back button is on a different UI
            backButton = GetComponentInParent<LobbyPlayerList>().backButton;
            AssignCustomNavigation(readyButton, backButton, OnSelectDirection.down);
            AssignCustomNavigation(spellButton1, backButton, OnSelectDirection.down);
            AssignCustomNavigation(spellButton2, backButton, OnSelectDirection.down);
            AssignCustomNavigation(spellButton3, backButton, OnSelectDirection.down);
            AssignCustomNavigation(spellButton4, backButton, OnSelectDirection.down);
            AssignCustomNavigation(removePlayerButton, backButton, OnSelectDirection.down);
            AssignCustomNavigation(backButton, spellButton1, OnSelectDirection.up);

            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        private void ValidateSpellSelection()
        {
            //seperate illegal and legal spells
            List<int> illegalSpellSlots = new List<int>();
            List<A_Spell> legalSpells = new List<A_Spell>();
            for(int i = 0; i < spells.Count - 1; i++)
            {
                if (!spellregistry.ValidateNormal(spells[i]))
                {
                    illegalSpellSlots.Add(i);
                }
                else
                {
                    legalSpells.Add(spells[i]);
                }
            }

            if (illegalSpellSlots.Count > 0)
            {
                //Replace illegal spells by legal spells that have not been chosen yet
                int currentSpellSlot = 0;
                //Search for first spell that has not already been taken
                for (int i = 0; i < spellregistry.spellList.Count; i++)
                {
                    bool isSpellLegal = true;
                    //Check against already taken spells
                    foreach (A_Spell takenSpell in legalSpells)
                    {
                        //if the registry spell has already been taken, flag as illegal an move to next spell in registry
                        if (spellregistry.spellList[i].spellID == takenSpell.spellID)
                        {
                            isSpellLegal = false;
                            break;
                        }
                    }

                    // Only if the spell is legal, move it into our spellslot
                    if (isSpellLegal)
                    {
                        //Otherwise, put it into the spellslot
                        spells[illegalSpellSlots[currentSpellSlot]] = spellregistry.spellList[i];

                        //And move to next illegal spell
                        currentSpellSlot++;
                        if (currentSpellSlot == illegalSpellSlots.Count)
                        {
                            break;
                        }
                    }
                }
            }
            
            if (!spellregistry.ValidateUltimate(spells[3]))
            {
                spells[3] = spellregistry.ultimateSpellList[0];
            }

            UpdateSpellButtons();
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

        public void SetUiInteractive(bool setActive)
        {
            Selectable[] selectables = transform.parent.GetComponentsInChildren<Selectable>();
            foreach(Selectable selectable in selectables)
            {
                selectable.interactable = setActive;
            }
            backButton.interactable = setActive;
        }

        public void UpdateSpellButtons()
        {
            spellButton1.transform.GetChild(0).GetComponent<Image>().sprite = spells[0].icon;
            spellButton2.transform.GetChild(0).GetComponent<Image>().sprite = spells[1].icon;
            spellButton3.transform.GetChild(0).GetComponent<Image>().sprite = spells[2].icon;
            spellButton4.transform.GetChild(0).GetComponent<Image>().sprite = spells[3].icon;
        }

        public void OnSpellButtonClicked(int spellButton)
        {
            mSpellToChange = spellButton;
            RectTransform spellSelectionPanel;
            spellSelectionPanel = LobbyManager.s_Singleton.mainMenu.spellSelectionPanel;
            spellSelectionPanel.GetComponent<SpellSelectionPanel>().player = this;
            //If spellslot 4 is selected, show ultimates
            spellSelectionPanel.GetComponent<SpellSelectionPanel>().showUltimates = spellButton == 3;
            spellSelectionPanel.gameObject.SetActive(true);
        }

        public void tradeSpells(A_Spell spell)
        {
           for(int i = 0; i < spells.Count; i++)
            {
                //Check if the chosen spell is already used
                if (spells[i] == spell && i != mSpellToChange)
                {
                    //If the spell is already used, just swap the spell to change with the old position of the new spell
                    spells[i] = spells[mSpellToChange];
                    break;
                }
            }
            spells[mSpellToChange] = spell;
            ValidateSpellSelection();
        }

        public void RandomizeSpells()
        {
            spells = spellregistry.generateRandomSpells();
            UpdateSpellButtons();
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
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;
                spellButton1.interactable = false;
                spellButton2.interactable = false;
                spellButton3.interactable = false;
                spellButton4.interactable = false;
                // Push chosen spells to server
                if (isLocalPlayer)
                {
                    ValidateSpellSelection();
                    //Save Spells to PlayerPrefs
                    PlayerPrefs.SetInt("SpellSlot0",spells[0].spellID);
                    PlayerPrefs.SetInt("SpellSlot1", spells[1].spellID);
                    PlayerPrefs.SetInt("SpellSlot2", spells[2].spellID);
                    PlayerPrefs.SetInt("SpellSlot3", spells[3].spellID);
                    PlayerPrefs.SetString("Playername", playerName);
                    PlayerPrefs.Save();
                    CmdSpellsChanged(spells[0].spellID, spells[1].spellID, spells[2].spellID, spells[3].spellID);
                }
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
                spellButton1.interactable = isLocalPlayer;
                spellButton2.interactable = isLocalPlayer;
                spellButton3.interactable = isLocalPlayer;
                spellButton4.interactable = isLocalPlayer;
            }
        }

        [Command]
        public void CmdSpellsChanged(int Spell1, int Spell2, int Spell3, int Spell4)
        {
            SpellRegistry spellregistry = Prototype.NetworkLobby.LobbyManager.s_Singleton.mainMenu.spellSelectionPanel.GetComponent<SpellSelectionPanel>().spellregistry;
            spells[0] = spellregistry.GetSpellByID(Spell1);
            spells[1] = spellregistry.GetSpellByID(Spell2);
            spells[2] = spellregistry.GetSpellByID(Spell3);
            spells[3] = spellregistry.GetSpellByID(Spell4);
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

        public void OnNameChanged(string str)
        {
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
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.mainMenu.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.mainMenu.countdownPanel.gameObject.SetActive(countdown != 0);

			LobbyManager.s_Singleton.mainMenu.GetRewiredPlayer().controllers.maps.SetMapsEnabled(false,"UI");
			LobbyManager.s_Singleton.mainMenu.GetRewiredPlayer().controllers.maps.SetMapsEnabled(true,"Default");
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
        public void CmdColorChange()
        {
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % Colors.Length;

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

            if (inUseIdx >= 0)
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
