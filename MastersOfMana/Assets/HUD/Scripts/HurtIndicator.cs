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
    private bool mIsInitialized = false;

    public void Awake()
    {
        GameManager.OnRoundStarted += RoundStarted;
        GameManager.OnLocalPlayerDead += localPlayerDead;
    }

    //// Use this for initialization
    void OnEnable()
    {
        if(!mIsInitialized)
        {
            Init();
        }
        localPlayerHealthScript.OnHealthChanged += HealthChanged;
        localPlayerHealthScript.OnDamageTaken += DamageTaken;
    }

    public void RoundStarted()
    {
        mMinAlpha = 0;
        Color col = sprite.color;
        col.a = 0;
        sprite.color = col;
        mRising = false;
        gameObject.SetActive(true);
    }

    public void Init()
    {
        localPlayerHealthScript = GameManager.instance.localPlayer.healthScript;
        mPlayerMaxHealth = localPlayerHealthScript.GetMaxHealth();
    }


    public void OnDisable()
    {
        if (localPlayerHealthScript)
        {
            localPlayerHealthScript.OnHealthChanged -= HealthChanged;
            localPlayerHealthScript.OnDamageTaken -= DamageTaken;
        }
    }

    public void OnDestroy()
    {
		GameManager.OnLocalPlayerDead -= localPlayerDead;
        GameManager.OnRoundStarted -= RoundStarted;
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
        gameObject.SetActive(false);
    }
}
