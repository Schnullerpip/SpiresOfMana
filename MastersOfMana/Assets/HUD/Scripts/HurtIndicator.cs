using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtIndicator : MonoBehaviour {

    private PlayerHealthScript localPlayerHealthScript;
    public Image sprite;
    public float flashSpeed = 0.01f;
    public float alphaIncrease = 0.05f;

    private bool inCoroutine = false;
    public float flashDuration = 0.5f;

    private bool mRising = false;
    private float mMinAlpha = 0.0f;

    // Use this for initialization
    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
    }

    public void Init()
    {
        localPlayerHealthScript = GameManager.instance.localPlayer.healthScript;
        // Set this UI Script in HealthScript so that HealtScript can update us
        localPlayerHealthScript.OnDamageTaken += Flash;
    }

    private void Update()
    {
        if(mRising)
        {
            Color col = sprite.color;
            col.a += flashSpeed;
            if(col.a >= 1)
            {
                col.a = 1;
                mRising = false;
            }
            sprite.color = col;
        }
        else
        {

        }
    }

    private void Flash()
    {
        if (!inCoroutine)
        {
            inCoroutine = true;
            StartCoroutine(DoFlash());
        }

    }

    IEnumerator DoFlash()
    {
        sprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        sprite.gameObject.SetActive(false);
        inCoroutine = false;
    }
}
