using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoHUD : MonoBehaviour
{
    public GameObject lavaImage;
    public GameObject energyImage;

    private UniformScaler mLavaScaler, mEnergyScaler;
    private ChangeImageColor mLavaColor, mEnergyColor;

    private LavaFloor lava;
    private PlatformSpiresEffect[] platforms;

    void Awake()
    {
        GameManager.OnPreGameAnimationFinished += RoundStarted;
        GameManager.OnRoundEnded += RoundEnded;

        mLavaScaler = lavaImage.GetComponent<UniformScaler>();
        mEnergyScaler = energyImage.GetComponent<UniformScaler>();

        mLavaColor = lavaImage.GetComponent<ChangeImageColor>();
        mEnergyColor = energyImage.GetComponent<ChangeImageColor>();
    }

    void OnDestroy()
    {
        GameManager.OnPreGameAnimationFinished -= RoundStarted;
        GameManager.OnRoundEnded -= RoundEnded;
    }

    void OnEnable()
    {
        lava = FindObjectOfType<LavaFloor>();
        platforms = FindObjectsOfType<PlatformSpiresEffect>();

        //register on events
        lava.OnLavaWillRise += InitializeLavaMotion;
        lava.OnLavaStoppedRising += UninitializeLavaMotion;

        foreach (var p in platforms)
        {
            p.OnLoadingZoneWillSpawn += InitializeEnergyMotion;
            p.OnLoadingZoneDisappeared += UninitializeEnergyMotion;
        }

    }

    void OnDisable()
    {
        //unregister on events
        lava.OnLavaWillRise -= InitializeLavaMotion;
        lava.OnLavaStoppedRising -= UninitializeLavaMotion;

        foreach (var p in platforms)
        {
            p.OnLoadingZoneWillSpawn -= InitializeEnergyMotion;
            p.OnLoadingZoneDisappeared -= UninitializeEnergyMotion;
        }
    }

    //EVENTS
    private void InitializeLavaMotion()
    {
        mLavaScaler.Play(true);
        mLavaColor.Change();
    }

	private void InitializeEnergyMotion(Color color)
    {
        mEnergyScaler.Play(true);
        mEnergyColor.Change(color);
    }

    private void UninitializeLavaMotion()
    {
        mLavaScaler.StopLoop();
        mLavaColor.Revert();
    }

    private void UninitializeEnergyMotion()
    {
        mEnergyScaler.StopLoop();
        mEnergyColor.Revert();
    }

    private void RoundStarted()
    {
        gameObject.SetActive(true);
    }

    private void RoundEnded()
    {
        gameObject.SetActive(false);
        mLavaScaler.Cancel();
        mLavaColor.Cancel();
        mEnergyScaler.Cancel();
        mEnergyColor.Cancel();
    }
}
