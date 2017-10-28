using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStateInvincible : EffectStateNoFallDamage {
    public EffectStateInvincible(PlayerScript player) : base(player) { }

    public override float CalculateDamage(float amount)
    {
        return 0;
    }
}
