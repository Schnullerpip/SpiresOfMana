using UnityEngine;
using UnityEngine.Networking;

public class EarthWallHealthScript : HealthScript {

    public int damagePerSecond = 1;

    private float counter = 0;

    private void OnEnable()
    {
        counter = 0;
    }

    public override void TakeDamage(int amount, System.Type typeOfDamageDealer)
    {
        base.TakeDamage(amount, typeOfDamageDealer);
        if(!IsAlive())
        {
            gameObject.SetActive(false);
            NetworkServer.UnSpawn(gameObject);
        }
    }

    private void Update()
    {
        if (isServer)
        {
            counter += Time.deltaTime;

            if (counter > 1.0f)
            {
                counter = 0;
                TakeDamage(damagePerSecond, GetType());
            }
        }
    }
}
