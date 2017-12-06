using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtIndicator : MonoBehaviour {

    private PlayerHealthScript localPlayerHealthScript;
    public Image sprite;
    public float flashSpeed = 0.06f;
    public float alphaIncreasePerPercentHealthLost = 0.75f;
    public AnimationCurve alphaLevel;

    private bool mRising = false;
    private float mMinAlpha = 0.0f;

    private float mPlayerMaxHealth;

    // Use this for initialization
    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
    }

    public void Init()
    {
        localPlayerHealthScript = GameManager.instance.localPlayer.healthScript;
        localPlayerHealthScript.OnHealthChanged += HealthChanged;
        localPlayerHealthScript.OnDamageTaken += DamageTaken;
        mPlayerMaxHealth = localPlayerHealthScript.GetMaxHealth();
        GameManager.OnLocalPlayerDead += localPlayerDead;
    }


    public void OnDisable()
    {
        if (localPlayerHealthScript)
        {
            localPlayerHealthScript.OnHealthChanged -= HealthChanged;
            localPlayerHealthScript.OnDamageTaken -= DamageTaken;
        }
		GameManager.OnLocalPlayerDead -= localPlayerDead;
    }

    private void Update()
    {
        if(mRising)
        {
            // we need to cache the color because we can't change the alpha value directly
            Color col = sprite.color;
            col.a += flashSpeed;
            if(col.a >= 1)
            {
                col.a = 1;
                mRising = false;
            }
            sprite.color = col;
        }
        else if(sprite.color.a > mMinAlpha)
        {
            // we need to cache the color because we can't change the alpha value directly
            Color col = sprite.color;
            col.a -= flashSpeed;
            sprite.color = col;
        }
    }

    private void HealthChanged(int newHealth)
    {
        //Calculate new minimum Alpha value
        mMinAlpha = alphaLevel.Evaluate(1 - (float)newHealth / mPlayerMaxHealth);
    }

    private void DamageTaken(int damage)
    {
        if(sprite.color.a <= mMinAlpha)
        {
            mRising = true;
        }
    }

    private void localPlayerDead()
    {
        sprite.enabled = false;
    }
}
