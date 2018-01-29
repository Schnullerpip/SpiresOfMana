using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyOptions : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    public Dropdown qualityDropdown;

    public UnityEngine.Audio.AudioMixer mixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle mutedToggle;
    public AudioSource sfxSampler;

    public Slider mouseSensitivity;
    public FloatReference aimSpeedRef;

    private Dictionary<int, Resolution> mResolutionDropdownMatch = new Dictionary<int, Resolution>();
    private Dictionary<string, int> mReverseResolutionDropdownMatch = new Dictionary<string,int>();
    private bool isInitialized = false;
    private bool valuesSaved = false;

    /// <summary>
    /// Applies the audio settings. 
    /// This should be called on game startup in order to ensure that the players preferences are applied before the first sound
    /// </summary>
    public void ApplySettings()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVol", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVol", 1);
        mutedToggle.isOn = PlayerPrefsExtended.GetBool("muted", false);
        mouseSensitivity.value = PlayerPrefs.GetFloat("mouse", 180f);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualitySettings", QualitySettings.GetQualityLevel()));

        OnAudioMutedChanged();
    }

    public void SetPlayerMouseSensitivity()
    {
        aimSpeedRef.value = mouseSensitivity.value;
    }

    public void Init()
    {
        List<string> screenResolutions = new List<string>();
        int dropdownIndex = 0;

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            string resolution = Screen.resolutions[i].ToString();
            resolution = resolution.Remove(resolution.LastIndexOf('@')-1);
            if(!mReverseResolutionDropdownMatch.ContainsKey(resolution))
            {
                screenResolutions.Add(resolution);
                mReverseResolutionDropdownMatch.Add(resolution, dropdownIndex);
                mResolutionDropdownMatch.Add(dropdownIndex, Screen.resolutions[i]);
                dropdownIndex++;
            }
        }
        resolutionDropdown.AddOptions(screenResolutions);
        List<string> qualitySettings = new List<string>();
        foreach(string setting in QualitySettings.names)
        {
            qualitySettings.Add(setting);
        }
        qualityDropdown.AddOptions(qualitySettings);
    }

    public void OnEnable()
    {
        if(!isInitialized)
        {
            isInitialized = true;
            Init();
        }
        string resolution = Screen.width + " x " + Screen.height; // We need this resolution because Screen.currentResolution only works for fullscreen
        if (mReverseResolutionDropdownMatch.ContainsKey(resolution))
        {
            resolutionDropdown.value = mReverseResolutionDropdownMatch[Screen.width + " x " + Screen.height];
        }
        fullscreenToggle.isOn = Screen.fullScreen;
        qualityDropdown.value = PlayerPrefs.GetInt("QualitySettings", QualitySettings.GetQualityLevel());
    }

    public void OnSave()
    {
        valuesSaved = true;
        Resolution resolution = mResolutionDropdownMatch[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        PlayerPrefsExtended.SetResolution("resolution", resolution);
        PlayerPrefsExtended.SetBool("fullscreen", fullscreenToggle.isOn);
        PlayerPrefs.SetFloat("musicVol", musicSlider.value);
        PlayerPrefs.SetFloat("sfxVol", sfxSlider.value);
        PlayerPrefsExtended.SetBool("muted", mutedToggle.isOn);
        PlayerPrefs.SetFloat("mouse", mouseSensitivity.value);
        PlayerPrefs.SetInt("QualitySettings", qualityDropdown.value);
        PlayerPrefs.Save();
        if (GameManager.instance.localPlayer)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Prototype.NetworkLobby.LobbyManager.s_Singleton.SimpleBackClbk();
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
			Prototype.NetworkLobby.LobbyManager.s_Singleton.Cancel ();

        }
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

    void OnDisable()
    {
        //Revert back to initial values!
        if(!valuesSaved)
        {
            RestoreInitialValues();
        }
    }

    private void RestoreInitialValues()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVol", 1);
        mixer.SetFloat("musicVol", Extensions.LinearToDecibel(musicSlider.value));

        sfxSlider.value = PlayerPrefs.GetFloat("sfxVol", 1);
        mixer.SetFloat("sfxVol", Extensions.LinearToDecibel(sfxSlider.value));

        mutedToggle.isOn = PlayerPrefsExtended.GetBool("muted", false);
        mixer.SetFloat("musicVol", Extensions.LinearToDecibel(mutedToggle.isOn ? 0 : musicSlider.value));
        mixer.SetFloat("sfxVol", Extensions.LinearToDecibel(mutedToggle.isOn ? 0 : sfxSlider.value));

        mouseSensitivity.value = PlayerPrefs.GetFloat("mouse", 180.0f);

        string screenKey = Screen.width + " x " + Screen.height;
        if (mReverseResolutionDropdownMatch.ContainsKey(screenKey))
        {
            resolutionDropdown.value = mReverseResolutionDropdownMatch[Screen.width + " x " + Screen.height];
        }
        else
        {
            string resolution = Screen.currentResolution.ToString();
            resolution = resolution.Remove(resolution.LastIndexOf('@') - 1);
            resolutionDropdown.value = mReverseResolutionDropdownMatch[resolution];
        }
        fullscreenToggle.isOn = Screen.fullScreen;

        qualityDropdown.value = PlayerPrefs.GetInt("QualitySettings", QualitySettings.GetQualityLevel());
    }
}

