using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : HealthScript {

    private PlayerScript mPlayer;

    public void Start()
    {
        mPlayer = GetComponent<PlayerScript>();
    }

    public override void TakeDamage(float amount) {
        mPlayer.mInputStateSystem.current.Hurt(amount);
    }
}
