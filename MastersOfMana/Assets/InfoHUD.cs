using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoHUD : MonoBehaviour
{

    [SerializeField] private Image LavaRisesImage;
    [SerializeField] private Image EnergyZoneImage;

    private LavaFloor lava;
    private PlatformSpiresEffect[] platforms;

	// Use this for initialization
	void Start () {
	    animationCurve.postWrapMode = WrapMode.PingPong;
        mCachedScaleLava = LavaRisesImage.transform.localScale;
        mCachedScaleEnergy = EnergyZoneImage.transform.localScale;
	}

    void Awake()
    {
        GameManager.OnRoundStarted += RoundStarted;
        GameManager.OnRoundEnded += RoundEnded;
    }

    void OnDestroy()
    {
        GameManager.OnRoundStarted -= RoundStarted;
        GameManager.OnRoundEnded -= RoundEnded;
    }

    void OnEnable()
    {
        lavaImageAaction = Idle;
        energyImageAction = Idle;

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
        lavaImageAaction = AnimateImage;
    }

    private void InitializeEnergyMotion()
    {
        energyImageAction = AnimateImage;
    }

    private void UninitializeLavaMotion()
    {
        lavaImageAaction = Idle;
        LavaRisesImage.transform.localScale = mCachedScaleLava;
    }

    private void UninitializeEnergyMotion()
    {
        energyImageAction = Idle;
        EnergyZoneImage.transform.localScale = mCachedScaleEnergy;
    }
	
	// Update is called once per frame
    private delegate void UpdateAction(Image image, Vector3 cachedScale);

    private UpdateAction lavaImageAaction, energyImageAction;
	void Update ()
	{
	    lavaImageAaction(LavaRisesImage, mCachedScaleLava);
	    energyImageAction(EnergyZoneImage, mCachedScaleEnergy);
	}

    [Header("ImageAnimation")]
    public AnimationCurve animationCurve;

    private Vector3 mCachedScaleLava, mCachedScaleEnergy;

    /// <summary>
    /// this is the method, that animates the image/icon so it is noticed and stated as 'active'
    /// whatever should happen to the image should be coded into this method!
    /// 
    /// right now the icon is scaled repeatedly to attract attention
    /// </summary>
    /// <param name="image">the image that is manipulated</param>
    /// <param name="cachedScale"> the original scale of the image</param>
    private void AnimateImage(Image image, Vector3 cachedScale)
    {
        Vector3 scale = image.gameObject.transform.localScale;
        scale.x = cachedScale.x*animationCurve.Evaluate(Time.time);
        scale.y = cachedScale.y*animationCurve.Evaluate(Time.time);
        scale.z = cachedScale.z*animationCurve.Evaluate(Time.time);

        image.transform.localScale = scale;
    }

    private void Idle(Image image, Vector3 cachedScale) {}


    private void RoundStarted()
    {
        gameObject.SetActive(true);
    }

    private void RoundEnded()
    {
        gameObject.SetActive(false);
    }

}
