using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// during casting we dont want to be able to induce another castroutine
/// </summary>
public class CastStateCasting : A_CastState {

    public CastStateCasting(PlayerScript player) : base(player) {}

    public override void Init()
    {
        //reset states so the animations behave strangely
        var anim = player.animator;
        anim.ResetTrigger("holdSpell");
        anim.ResetTrigger("resolveSpell");
        anim.SetBool("isCasting", false);//kinda useless, since its set again right after...

        //apply the spells cooldown -> even if the castprocedure is interrupted, the cooldown will be applied
        var spellslot = player.Currentspell();
        spellslot.cooldown = spellslot.spell.castDurationInSeconds;
        ResetCastDurationCount();
        //invoke casting animation
        anim.SetBool("isCasting", true);

    }

    public override void IncrementCastDuration()
    {
        castDurationCount += Time.deltaTime;
    }

    public override void ReduceCooldowns() {}

    public override void CastCmdSpell() {}

    public override void ResolveCmdSpell()
    {
        player.CmdResolveSpell(CalculateAimDirection());
    }
}
