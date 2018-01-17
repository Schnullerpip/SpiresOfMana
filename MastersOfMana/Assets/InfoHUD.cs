using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoHUD : MonoBehaviour
{

    [SerializeField] private Image LavaRisesImage;
    [SerializeField] private Image EnergyZone;

    private LavaFloor lava;
    private EnergyZoneSystem ezs;

	// Use this for initialization
	void Start () {
        //LavaRisesImage.gameObject.SetActive(false);
        //EnergyZone.gameObject.SetActive(false);

	    animationCurve.postWrapMode = WrapMode.PingPong;
        mCachedScaleLava = LavaRisesImage.transform.localScale;
        mCachedScaleEnergy = EnergyZone.transform.localScale;
	}

    void OnEnable()
    {
        lavaImageAaction = Idle;
        energyImageAction = Idle;

        lava = FindObjectOfType<LavaFloor>();
        ezs = FindObjectOfType<EnergyZoneSystem>();

        //register on events
        lava.OnLavaWillRise += InitializeLavaMotion;
        ezs.OnLoadingZoneWillSpawn += InitializeEnergyMotion;

        lava.OnLavaStoppedRising += UninitializeLavaMotion;
        ezs.OnLoadingZoneDisappeared += UninitializeEnergyMotion;
    }

    void OnDisable()
    {
        //unregister on events
        lava.OnLavaWillRise -= InitializeLavaMotion;
        ezs.OnLoadingZoneWillSpawn -= InitializeEnergyMotion;

        lava.OnLavaStoppedRising -= UninitializeLavaMotion;
        ezs.OnLoadingZoneDisappeared -= UninitializeEnergyMotion;
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
    }

    private void UninitializeEnergyMotion()
    {
        energyImageAction = Idle;
    }
	
	// Update is called once per frame
    private delegate void UpdateAction(Image image, Vector3 cachedScale);

    private UpdateAction lavaImageAaction, energyImageAction;
	void Update ()
	{
	    lavaImageAaction(LavaRisesImage, mCachedScaleLava);
	    energyImageAction(EnergyZone, mCachedScaleEnergy);
	}

    [Header("ImageAnimation")]
    public AnimationCurve animationCurve;

    private Vector3 mCachedScaleLava, mCachedScaleEnergy;
    private void AnimateImage(Image image, Vector3 cachedScale)
    {
        Vector3 scale = image.gameObject.transform.localScale;
        scale.x = cachedScale.x*animationCurve.Evaluate(Time.time);
        scale.y = cachedScale.y*animationCurve.Evaluate(Time.time);
        scale.z = cachedScale.z*animationCurve.Evaluate(Time.time);

        image.transform.localScale = scale;
    }

    private void Idle(Image image, Vector3 cachedScale) {}


}
