﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : HealthScript {

    private PlayerScript player;

    public void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    public override void TakeDamage(float amount) {
        player.inputStateSystem.current.Hurt(amount);
    }
}