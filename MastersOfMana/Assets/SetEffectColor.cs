using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEffectColor : MonoBehaviour {

    public Color Color = Color.red;

    private Color previousColor;

    void OnEnable()
    {
        UpdateColor();
    }

    void Update()
    {
        if (previousColor != Color)
        {
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        var particleTrail = GetComponentsInChildren<RFX4_ParticleTrail>(true);
        foreach(var trail in particleTrail)
        {
            var keys = trail.ColorOverLifeTime.colorKeys;
            keys[0].color = Color;
            trail.ColorOverLifeTime.SetKeys(keys, trail.ColorOverLifeTime.alphaKeys);
        }

        var particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var system in particleSystems)
        {
            ParticleSystem.MinMaxGradient color = new ParticleSystem.MinMaxGradient();
            color.mode = ParticleSystemGradientMode.Color;
            color.color = Color;

            //Assign the color to your particle
            ParticleSystem.MainModule main = system.main;
            main.startColor = color;
        }

        var lights = GetComponentsInChildren<Light>(true);
        foreach (var light in lights)
        {
            light.color = Color;
        }

        previousColor = Color;
    }
}
