using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.NetworkLobby
{
    public class LobbyOptions : MonoBehaviour
    {
        public Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;
        private Dictionary<int, Resolution> mResolutionDropdownMatch = new Dictionary<int, Resolution>();

        public void Start()
        {
            List<string> screenResolutions = new List<string>();
            for(int i = 0; i < Screen.resolutions.Length; i++)
            {
                screenResolutions.Add(Screen.resolutions[i].ToString());
                mResolutionDropdownMatch.Add(i, Screen.resolutions[i]);
            }
            resolutionDropdown.AddOptions(screenResolutions);
            resolutionDropdown.value = screenResolutions.FindIndex(x => x.Contains(Screen.currentResolution.ToString()));
            fullscreenToggle.isOn = Screen.fullScreen;
        }

        public void OnSave()
        {
            Resolution resolution = mResolutionDropdownMatch[resolutionDropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
            PlayerPrefsExtended.SetResolution("resolution", resolution);
            PlayerPrefsExtended.SetBool("fullscreen", fullscreenToggle.isOn);
            PlayerPrefs.Save();
            LobbyManager.s_Singleton.SimpleBackClbk();
        }
    }
}
