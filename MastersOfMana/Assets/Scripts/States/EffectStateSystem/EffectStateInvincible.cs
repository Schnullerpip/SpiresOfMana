using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStateInvincible : EffectStateNoFallDamage {
    public EffectStateInvincible(PlayerScript player) : base(player) { }

    public override int CalculateDamage(int amount, System.Type dealer)
    {
        return 0;
    }
}
