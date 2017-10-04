using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStateNormal : A_EffectState {
    private PlayerHealthScript mHealth;
    public EffectStateNormal(PlayerScript player) : base(player) {
        mHealth = player.GetComponent<PlayerHealthScript>();
    }

    /*behaviour distinction
     * those are keptempty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */

    //TODO...
    public override void Hurt(float amount)
    {
    }
}
