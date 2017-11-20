using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStateNoFallDamage : A_EffectState {

    public EffectStateNoFallDamage(PlayerScript player) : base(player) { }

    public override int CalculateFallDamage(int amount)
    {
        return 0;
    }
}
