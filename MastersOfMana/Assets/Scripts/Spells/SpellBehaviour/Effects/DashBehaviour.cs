using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBehaviour : A_SpellBehaviour {

    IEnumerator SlowDownAgain(PlayerScript caster) {
        //cache the original speed
        float originalMovementAcceleration = caster.speed;
        //dash the player
        caster.speed += 10;
        //wait for one second
        yield return new WaitForSeconds(1);
        //slow the player down again
        caster.speed = originalMovementAcceleration;
    }

    public override void Execute(PlayerScript caster)
    {
        caster.StartCoroutine(SlowDownAgain(caster));
    }
}
