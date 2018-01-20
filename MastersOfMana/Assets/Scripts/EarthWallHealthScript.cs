using UnityEngine;
using UnityEngine.Networking;

public class EarthWallHealthScript : HealthScript
{

    private EarthwallBehaviour mEarthWall;

    public int damagePerSecond = 1;

    new void Start()
    {
        mEarthWall = GetComponent<EarthwallBehaviour>();
    }

    private void OnEnable()
    {
        ResetObject();
    }

    public override void TakeDamage(int amount, PlayerScript damageDealer , System.Type typeOfDamageDealer)
    {
        base.TakeDamage(amount, damageDealer, typeOfDamageDealer);
        if(!IsAlive())
        {
            mEarthWall.EndSpell();
        }
    }

    private void Update()
    {
    }
}
