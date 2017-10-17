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

    /// <summary>
    /// will invoke the spell that is maped at spellslot 1
    /// </summary>
    public override void CastCmdSpellslot_1() {
        player.CmdSpellslot_1(CalculateAimDirection());
    }

    /// <summary>
    /// will invoke the spell that is maped at spellslot 2
    /// </summary>
    public override void CastCmdSpellslot_2() {
        player.CmdSpellslot_2(CalculateAimDirection());
    }

    /// <summary>
    /// will invoke the spell that is maped at spellslot 3
    /// </summary>
    public override void CastCmdSpellslot_3() {
        player.CmdSpellslot_3(CalculateAimDirection());
    }
}
