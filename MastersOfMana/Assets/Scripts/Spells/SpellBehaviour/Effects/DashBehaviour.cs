using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBehaviour : A_SpellBehaviour {

    IEnumerator SlowDownAgain(PlayerScript caster) {
        //cache the original speed
        float originalMovementAcceleration = caster.speed;
        Debug.Log("originalMovementAcceleration: " + caster.speed);
        //dash the player
        caster.speed += 10;
        Debug.Log("enhanced speed: " + caster.speed);
        //wait for one second
        yield return new WaitForSeconds(1);
        //slow the player down again
        caster.speed = originalMovementAcceleration;
        Debug.Log("back to normal speed: " + caster.speed);
    }

    public override void Execute(PlayerScript caster)
    {
        caster.StartCoroutine(SlowDownAgain(caster));
    }
}
