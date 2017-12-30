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
        var transformMotion = GetComponentInChildren<RFX4_TransformMotion>(true);
        var rayCastCollision = GetComponentInChildren<RFX4_RaycastCollision>(true);
        var particleTrail = GetComponentsInChildren<RFX4_ParticleTrail>(true);
        var hue = RFX4_ColorHelper.ColorToHSV(Color).H;
        RFX4_ColorHelper.ChangeObjectColorByHUE(gameObject, hue);
        if (transformMotion != null) transformMotion.HUE = hue;
        if (rayCastCollision != null) rayCastCollision.HUE = hue;
        foreach(var trail in particleTrail)
            if(trail != null) trail.ColorOverLifeTime.colorKeys[0].color = Color;
        previousColor = Color;
    }
}
