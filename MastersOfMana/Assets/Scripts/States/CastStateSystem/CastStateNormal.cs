using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateNormal : A_CastState{
    public CastStateNormal(PlayerScript player) : base(player) { }

    public override void ReduceCooldowns()
    {
        if ((player.spellslot[0].cooldown -= Time.deltaTime) < 0)
        {
            player.spellslot[0].cooldown = 0;
        }
        if ((player.spellslot[1].cooldown -= Time.deltaTime) < 0)
        {
            player.spellslot[1].cooldown = 0;
        }
        if ((player.spellslot[2].cooldown -= Time.deltaTime) < 0)
        {
            player.spellslot[2].cooldown = 0;
        }
    }

    public override void Init()
    {
        player.FlushSpellroutines();
        ResetCastDurationCount();

        //tell player that its animator should no longer hold the state 'isHolding'
        player.animator.SetBool(AnimationLiterals.ANIMATION_BOOL_CASTING, false);
        player.animator.ResetTrigger(AnimationLiterals.ANIMATION_TRIGGER_HOLD);
    }
}
