using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolorParticleSystems : ARecolorGradient
{
    public ParticleSystem[] particleSystems;

    protected override void Init()
    {
        mOriginalGradients = new Gradient[particleSystems.Length];

        for (int i = 0; i < mOriginalGradients.Length; ++i)
        {
            mOriginalGradients[i] = particleSystems[i].main.startColor.gradient;
        }
    }

    public override void ChangeColor()
    {
        ChangeColorTo(secondaryGradient);
    }

    public override void ChangeColor(float t)
    {
        ChangeColorTo(secondaryGradient, t);
    }

    public override void RevertColor()
    {
        ParticleSystem.MainModule main;
        ParticleSystem.MinMaxGradient grad;

        for (int i = 0; i < particleSystems.Length; ++i)
        {
            main = particleSystems[i].main;
            grad = main.startColor;
            grad.gradient = mOriginalGradients[i];
            main.startColor = grad;
        }
    }

    public override void RevertColor(float t)
    {
        RevertColor();
    }

    public override void ChangeColorTo(Gradient gradient)
    {
        ParticleSystem.MainModule main;
        ParticleSystem.MinMaxGradient grad;

        for (int i = 0; i < particleSystems.Length; ++i)
        {
            main = particleSystems[i].main;
            grad = main.startColor;
            grad.gradient = gradient;
            main.startColor = grad;
        }
    }

    public override void ChangeColorTo(Gradient gradient, float t)
    {
        ChangeColorTo(gradient);
    }
}