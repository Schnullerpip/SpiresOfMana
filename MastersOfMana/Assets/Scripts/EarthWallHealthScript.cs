using UnityEngine;
using UnityEngine.Networking;

public class EarthWallHealthScript : HealthScript {

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if(!IsAlive())
        {
            gameObject.SetActive(false);
            NetworkServer.UnSpawn(gameObject);
        }
    }
}
