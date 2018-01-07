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

        public UnityEngine.Audio.AudioMixer mixer;
        public Slider musicSlider;
        public Slider sfxSlider;
        public Toggle mutedToggle;
        public AudioSource sfxSampler;

        private Dictionary<int, Resolution> mResolutionDropdownMatch = new Dictionary<int, Resolution>();
        private Dictionary<Resolution, int> mReverseResolutionDropdownMatch = new Dictionary<Resolution,int>();
        private bool isInitialized = false;

        /// <summary>
        /// Applies the audio settings. 
        /// This should be called on game startup in order to ensure that the players preferences are applied before the first sound
        /// </summary>
        public void ApplyAudioSettings()
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVol", 1);
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVol", 1);
            mutedToggle.isOn = PlayerPrefsExtended.GetBool("muted", false);

            OnAudioMutedChanged();
        }

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
        }

        public void OnSave()
        {
            Resolution resolution = mResolutionDropdownMatch[resolutionDropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
            PlayerPrefsExtended.SetResolution("resolution", resolution);
            PlayerPrefsExtended.SetBool("fullscreen", fullscreenToggle.isOn);
            PlayerPrefs.SetFloat("musicVol", musicSlider.value);
            PlayerPrefs.SetFloat("sfxVol", sfxSlider.value);
            PlayerPrefsExtended.SetBool("muted", mutedToggle.isOn);
            PlayerPrefs.Save();
            LobbyManager.s_Singleton.SimpleBackClbk();
        }

        public void OnMusicSliderChanged()
        {
            if (!mutedToggle.isOn)
            {
                mixer.SetFloat("musicVol", Extensions.LinearToDecibel(musicSlider.value));
            }
        }

        public void OnSfxSliderChanged()
        {
            if (!mutedToggle.isOn)
            {
                mixer.SetFloat("sfxVol", Extensions.LinearToDecibel(sfxSlider.value));
            }
        }

        public void OnAudioMutedChanged()
        {
            mixer.SetFloat("musicVol", Extensions.LinearToDecibel(mutedToggle.isOn ? 0 : musicSlider.value));
            mixer.SetFloat("sfxVol", Extensions.LinearToDecibel(mutedToggle.isOn ? 0 : sfxSlider.value));
        }

        public void ContinousSampleSFX()
        {
            if (!sfxSampler.isPlaying)
            {
                SampleSFX();
            }
        }

		public void SampleSFX()
        {
            sfxSampler.Stop();
            sfxSampler.Play();
        }
    }
}
