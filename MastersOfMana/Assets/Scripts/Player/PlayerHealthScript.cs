using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : HealthScript {

    private PlayerScript mPlayer;

    public override void Start()
    {
        mPlayer = GetComponent<PlayerScript>();
        base.Start();
    }

    public override void TakeDamage(float amount) {
        base.TakeDamage(mPlayer.effectStateSystem.current.Hurt(amount));

        if (!IsAlive()) {
            //this mPlayer is dead!!! tell the Gamemanager, that one is down
            GameManager.PlayerDown();
        }
    }
}
