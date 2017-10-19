using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateNormal : A_CastState{
    public CastStateNormal(PlayerScript player) : base(player) { }

    public override void ReduceCooldowns()
    {
        if ((player.spellSlot_1.cooldown -= Time.deltaTime) < 0)
        {
            player.spellSlot_1.cooldown = 0;
        }
        if ((player.spellSlot_2.cooldown -= Time.deltaTime) < 0)
        {
            player.spellSlot_2.cooldown = 0;
        }
        if ((player.spellSlot_3.cooldown -= Time.deltaTime) < 0)
        {
            player.spellSlot_3.cooldown = 0;
        }
    }

    public override void Init()
    {
        player.FlushSpellroutines();
        ResetCastDurationCount();

        //tell player that its animator should no longer hold the state 'isHolding'
        player.animator.SetBool("isCasting", false);
    }

    /// <summary>
    /// will invoke the spell that is maped at spellslot 1
    /// </summary>
    public override void CastCmdSpell() {
        player.CmdCastSpell();
    }

    /// <summary>
    /// in the normal state a spell cant resolve, since none was cast...
    /// </summary>
    public override void ResolveCmdSpell() {}
}
