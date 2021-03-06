﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateResolving : A_CastState {

    public CastStateResolving(PlayerScript player) : base(player) { }

    public override void Init()
    {
        //set the cooldown of the current spellslot
		player.GetPlayerSpells().GetCurrentspell().ResetCooldown();
        player.GetPlayerAnimation().Cast(player.GetPlayerSpells().GetCurrentspell().spell.castAnimationID);
    }
}