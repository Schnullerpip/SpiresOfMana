using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.NetworkLobby
{
    public class LobbyOptions : MonoBehaviour
    {
        public Dropdown controlScheme;

        public void OnEnable()
        {
            LobbyManager.s_Singleton.SetCancelDelegate(LobbyManager.s_Singleton.SimpleBackClbk);
            controlScheme.value = PlayerPrefs.GetInt("ControlScheme");
        }

        public void OnSave()
        {
            PlayerPrefs.SetInt("ControlScheme", controlScheme.value);
            PlayerPrefs.Save();
            LobbyManager.s_Singleton.Cancel();
        }
    }
}
