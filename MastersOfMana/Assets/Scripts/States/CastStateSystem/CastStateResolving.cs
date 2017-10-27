using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateResolving : A_CastState {

    public CastStateResolving(PlayerScript player) : base(player) { }

    public override void Init()
    {
        //set the cooldown of the current spellslot
        player.Currentspell().cooldown = player.Currentspell().spell.coolDownInSeconds;

        var anim = player.animator;

        //tell the players animator to start the resolve animation
        anim.SetTrigger(AnimationLiterals.ANIMATION_TRIGGER_RESOLVE);

        //TODO clear with Lukas -> what do i need to do?!?!??!
        //anim.ResetTrigger(AnimationLiterals.ANIMATION_TRIGGER_HOLD);
        //tell player that its animator should no longer hold the state 'isHolding'
        //anim.SetBool(AnimationLiterals.ANIMATION_BOOL_CASTING, false);
    }
}
