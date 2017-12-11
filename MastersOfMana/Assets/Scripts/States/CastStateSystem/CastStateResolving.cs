using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateResolving : A_CastState {

    public CastStateResolving(PlayerScript player) : base(player) { }

    public override void Init()
    {
		player.GetPlayerAnimation().Cast();

        //set the cooldown of the current spellslot
        player.GetPlayerSpells().GetCurrentspell().cooldown = player.GetPlayerSpells().GetCurrentspell().spell.coolDownInSeconds;
    }
}