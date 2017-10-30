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
        private bool initialized = false;
        private List<Button> spellButtons = new List<Button>();

    private void OnEnable()
        {
            if (initialized)
                return;

            initialized = true;

            for (int i = 0; i < spellregistry.SpellList.Count; i++)
            {
                A_Spell spell = spellregistry.SpellList[i];
                Button spellButton = GameObject.Instantiate(spellButtonPrefab);
                Image image = spellButton.transform.GetChild(0).GetComponent<Image>();
                if (image)
                {
                    image.sprite = spell.icon;
                }
                spellButton.transform.SetParent(spellList);
                spellButton.transform.localScale = new Vector3(1, 1, 1);
                spellButton.onClick.AddListener(delegate { player.tradeSpells(spell); });
                spellButtons.Add(spellButton);
            }
            // This will make sure the first spell is highlighted when opening
            GetComponentInParent<MultipleMenuInput>().firstSelected = spellButtons[0].gameObject;

            player.SetUiInteractive(false);

            //Assign correct Navigation for the spellButtons
            //for(int i = 0; i < spellButtons.Count; i++)
            //{
            //    //if(i==0)
            //    //{
            //    //    AssignCustomNavigation()
            //    //}
            //}
            LobbyManager.s_Singleton.SetCancelDelegate(GoBack);
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

        public void GoBack()
        {
            gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            player.SetUiInteractive(true);
        }
    }
}
