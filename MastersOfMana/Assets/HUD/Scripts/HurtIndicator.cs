﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtIndicator : MonoBehaviour {

    private PlayerHealthScript localPlayerHealthScript;
    public Image sprite;
    public float flashDuration = 0.5f;
    private Coroutine mflashCorutine;

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

    private void Flash()
    {
        if(mflashCorutine != null)
        {
            StopCoroutine(mflashCorutine);
        }
        mflashCorutine = StartCoroutine(DoFlash());

    }

    IEnumerator DoFlash()
    {
        sprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        sprite.gameObject.SetActive(false);
    }
}
