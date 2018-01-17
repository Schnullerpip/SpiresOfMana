using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEmissionFade : MonoBehaviour
{

    public ParticleSystem Particles;
    private ParticleSystem.EmissionModule EmissionModule;

    public float FadeAfterSeconds;

    public float FadeRate;

    /// <summary>
    /// will be used to reset the particle system
    /// </summary>
    private float CachedEmission;
    
	// Use this for initialization
	void Start () {
	}

    void Awake()
    {
	    if (!Particles)
	    {
	        Particles = GetComponent<ParticleSystem>();
	        if (!Particles)
	        {
	            throw new MissingMemberException("No Particle system was attached or could be found on the object");
	        }
	    }

	    if (Particles)
	    {
	        CachedEmission = Particles.emission.rateOverTime.constant;
	    }

	    if (FadeAfterSeconds <= 0)
	    {
	        Debug.LogWarning("FadeAfterSeconds value of: " + FadeAfterSeconds + " seems suspicious, you might wanna rethink that");
	    }
	    if (FadeRate >= 1 || FadeRate <= 0)
	    {
	        Debug.LogWarning("FadeRate value of: " + FadeRate + " seems suspicious, you might wanna rethink that");
	    }

        EmissionModule = Particles.emission;
    }

    void OnEnable()
    {
        mTimeCount = 0;
        EmissionModule.rateOverTime = CachedEmission;
    }

    private float mTimeCount;
    void Update()
    {
        mTimeCount += Time.deltaTime;
        if (mTimeCount >= FadeAfterSeconds)
        {
            //var rate = emission.rateOverTime.constant * FadeRate;
            EmissionModule.rateOverTime = EmissionModule.rateOverTime.constant*FadeRate;
        }
    }
}

