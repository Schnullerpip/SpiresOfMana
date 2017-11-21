using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Prototype.NetworkLobby {
    public class SpellSelectionPanel : MonoBehaviour {

        public SpellRegistry spellregistry;
        public RectTransform spellList;
        public Button spellButtonPrefab;
        public LobbyPlayer player;
        public bool showUltimates = false;
        private List<Button> spellButtons = new List<Button>();

        private void OnEnable()
            {
                player.SetUiInteractive(false);
                LobbyManager.s_Singleton.SetCancelDelegate(GoBack);

                Init();
                spellButtons[0].OnSelect(null);
            }

        public void Init()
        {
            List<A_Spell> spells = spellregistry.SpellList;
            if (showUltimates)
            {
                spells = spellregistry.UltimateSpellList;
            }
            for (int i = 0; i < spells.Count; i++)
            {
                A_Spell spell = spells[i];
                Button spellButton = GameObject.Instantiate(spellButtonPrefab);
                Image image = spellButton.transform.GetChild(0).GetComponent<Image>();
                if (image)
                {
                    image.sprite = spell.icon;
                }
                spellButton.transform.SetParent(spellList);
                spellButton.transform.localScale = new Vector3(1, 1, 1);
                spellButton.onClick.AddListener(delegate { OnClickSpellButton(spell); });
                spellButtons.Add(spellButton);
            }
            // This will make sure the first spell is highlighted when opening
            GetComponentInParent<MultipleMenuInput>().firstSelected = spellButtons[0].gameObject;
        }

        private void OnClickSpellButton(A_Spell spell)
        {
            player.tradeSpells(spell);
            Cancel();
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

        public void Cancel()
        {
            LobbyManager.s_Singleton.Cancel();
        }

        public void GoBack()
        {
            player.GetComponentInParent<MultipleMenuInput>().highlightFirstSelected();
            gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            player.SetUiInteractive(true);
            foreach(Button button in spellButtons)
            {
                Destroy(button.gameObject);
            }
            spellButtons.Clear();
        }
    }
}
