using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateNormal : A_CastState{
    public CastStateNormal(PlayerScript player) : base(player) { }

    public override void ReduceCooldowns()
    {
		PlayerSpells playerSpells = player.GetPlayerSpells();

		for (int i = 0; i < 3; i++) 
		{
			if(playerSpells.spellslot[i].ReduceCooldown(Time.deltaTime))
			{
				if(player.isLocalPlayer)
				{
					playerSpells.PlayCooldownDoneSFX();
				}
			}
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