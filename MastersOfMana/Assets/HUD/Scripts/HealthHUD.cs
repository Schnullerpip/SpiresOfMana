﻿using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    private PlayerHealthScript localPlayerHealthScript;
    public Text healthText;
    public Canvas canvas;

    // Use this for initialization
    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
    }

    public void Init()
    {
        localPlayerHealthScript = GameManager.instance.localPlayer.healthScript;
        // Set this UI Script in HealthScript so that HealtScript can update us
        localPlayerHealthScript.healthHUD = this;
        SetHealth(localPlayerHealthScript.GetCurrentHealth());
        canvas = GetComponentInParent<Canvas>();
        canvas.enabled = true;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
        //canvas.enabled = false;
    }

    public void SetHealth(float health)
    {
        healthText.text = "Health: " + (int)health;
    }
}