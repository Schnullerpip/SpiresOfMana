using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateNormal : A_CastState{
    public CastStateNormal(PlayerScript player) : base(player) { }

    public override void ReduceCooldowns()
    {
        if ((player.GetPlayerSpells().spellslot[0].cooldown -= Time.deltaTime) < 0)
        {
            player.GetPlayerSpells().spellslot[0].cooldown = 0;
        }
        if ((player.GetPlayerSpells().spellslot[1].cooldown -= Time.deltaTime) < 0)
        {
            player.GetPlayerSpells().spellslot[1].cooldown = 0;
        }
        if ((player.GetPlayerSpells().spellslot[2].cooldown -= Time.deltaTime) < 0)
        {
            player.GetPlayerSpells().spellslot[2].cooldown = 0;
        }
    }

    public override void UpdateSynchronized()
    {
        ReduceCooldowns();
    }

    public override void Init()
    {
        player.GetPlayerSpells().FlushSpellroutines();
        ResetCastDurationCount();
    }
}