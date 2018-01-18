using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadingZoneUltimate : LoadingZone
{
    public float ultimateEnergyPerSecond = 1;
	public float tickDuration = 1;

    public override IEnumerator LoadingZoneEffect(PlayerScript player)
    {
        var spells = player.GetComponentInParent<PlayerSpells>();
        while (enabled)
        {
            if(spells.ultimateEnergy > 0)
            {
				yield return new WaitForSeconds(tickDuration);            
            }

            if(!GameManager.instance.isUltimateActive)
            {
                spells.ultimateEnergy += ultimateEnergyPerSecond;
            }
        }
    }
}
