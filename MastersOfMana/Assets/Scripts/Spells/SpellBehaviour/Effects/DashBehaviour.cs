using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specifies, how the dash spell works 
/// it takes the casters original speed and enhances it for a distinct period of time, then reverts the speed value of the player to the previously cached value
/// !!! THIS COULD BE BETTER -> there probably should be an original speed constant in the playerscript class that is referenced instead of caching a value on the effect start
/// </summary>
public class DashBehaviour : A_EffectBehaviour {

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
