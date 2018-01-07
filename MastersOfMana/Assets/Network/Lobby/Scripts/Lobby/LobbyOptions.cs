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
        public Slider volumeSlider;
        public Toggle mutedToggle;
        private Dictionary<int, Resolution> mResolutionDropdownMatch = new Dictionary<int, Resolution>();
        private Dictionary<Resolution, int> mReverseResolutionDropdownMatch = new Dictionary<Resolution,int>();
        private bool isInitialized = false;
        private bool initialIsMuted;
        private float initialVolume;
        private bool valuesSaved = false;

        public void Init()
        {
             List<string> screenResolutions = new List<string>();
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                screenResolutions.Add(Screen.resolutions[i].ToString());
                mResolutionDropdownMatch.Add(i, Screen.resolutions[i]);
                mReverseResolutionDropdownMatch.Add(Screen.resolutions[i], i);
            }
            resolutionDropdown.AddOptions(screenResolutions);
        }

        public void OnEnable()
        {
            if(!isInitialized)
            {
                isInitialized = true;
                Init();
            }
            resolutionDropdown.value = mReverseResolutionDropdownMatch[Screen.currentResolution];
            fullscreenToggle.isOn = Screen.fullScreen;
            initialVolume = volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
            initialIsMuted = mutedToggle.isOn = PlayerPrefsExtended.GetBool("muted", false);
        }

        public void OnSave()
        {
            valuesSaved = true;
            Resolution resolution = mResolutionDropdownMatch[resolutionDropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
            PlayerPrefsExtended.SetResolution("resolution", resolution);
            PlayerPrefsExtended.SetBool("fullscreen", fullscreenToggle.isOn);
            PlayerPrefs.SetFloat("Volume", volumeSlider.value);
            PlayerPrefsExtended.SetBool("muted", mutedToggle.isOn);
            PlayerPrefs.Save();
            if (GameManager.instance.localPlayer)
            {
                gameObject.SetActive(false);
            }
            else
            {
                LobbyManager.s_Singleton.SimpleBackClbk();
            }
        }

        public void OnBack()
        {
            if(GameManager.instance.localPlayer)
            {
                gameObject.SetActive(false);
            }
            else
            {
                LobbyManager.s_Singleton.SimpleBackClbk();
            }
            AudioListener.volume = initialIsMuted ? 0 : initialVolume;
        }

        public void OnVolumeSliderChanged()
        {
            if (!mutedToggle.isOn)
            {
                AudioListener.volume = volumeSlider.value;
            }
        }

        public void OnAudioMutedChanged()
        {
            AudioListener.volume = mutedToggle.isOn ? 0 : volumeSlider.value;
        }

        void OnDisable()
        {
            //Revert back to initial values!
            if(!valuesSaved)
            {
                OnBack();
            }
        }
    }
}
