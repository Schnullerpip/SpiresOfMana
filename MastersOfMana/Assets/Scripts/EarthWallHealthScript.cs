using UnityEngine;
using UnityEngine.Networking;

public class EarthWallHealthScript : HealthScript {

    public override void TakeDamage(int amount, System.Type typeOfDamageDealer)
    {
        base.TakeDamage(amount, typeOfDamageDealer);
        if(!IsAlive())
        {
            gameObject.SetActive(false);
            NetworkServer.UnSpawn(gameObject);
        }
    }
}
