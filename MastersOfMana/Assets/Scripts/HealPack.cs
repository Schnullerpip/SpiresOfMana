using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealPack : LoadingZone
{
    public int healPerTick = 2;
    public float tickDuration = 0.5f;

    public override IEnumerator LoadingZoneEffect(PlayerScript player)
    {
        var health = player.GetComponent<PlayerHealthScript>();
        while (enabled)
        {
            if (health.GetCurrentHealth() < health.GetMaxHealth())
            {
                health.TakeHeal(healPerTick);
            }
            yield return new WaitForSeconds(tickDuration);
        }
    }
}
