﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : HealthScript {

    private PlayerScript mPlayer;
    public HealthHUD healthHUD;

    public override void Start()
    {
        mPlayer = GetComponent<PlayerScript>();
        base.Start();
    }

    public override void TakeDamage(float amount) {
        bool hasBeenAlive = IsAlive();
        base.TakeDamage(mPlayer.effectStateSystem.current.CalculateDamage(amount));

        if (!IsAlive() && hasBeenAlive) {
            //this mPlayer is dead!!! tell the Gamemanager, that one is down
            GameManager.instance.PlayerDown();
        }
    }

    public void TakeFallDamage(float amount)
    {
        TakeDamage(mPlayer.effectStateSystem.current.CalculateFallDamage(amount));
    }

    public override void OnHealthChanged(float newHealth)
    {
        //At game start healthHud might not be available yet
        if(healthHUD)
        {
            healthHUD.SetHealth(newHealth);
        }
    }
}
