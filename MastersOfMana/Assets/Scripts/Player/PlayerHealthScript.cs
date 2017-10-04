using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : HealthScript {

    private PlayerScript player;

    public void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    public override void TakeDamage(float amount) {
        player.effectStateSystem.current.Hurt(amount);

        if (GetCurrentHealth() <= 0) {
            //this player is dead!!! tell the Gamemanager, that one is down
            //TODO send Cmd to GameManager
        }
    }
}
