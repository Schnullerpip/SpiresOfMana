using UnityEngine;
using UnityEngine.Networking;

public class EarthWallHealthScript : HealthScript
{

    private EarthwallBehaviour mEarthWall;

    public int damagePerSecond = 1;

    private float counter = 0;

    void Start()
    {
        mEarthWall = GetComponent<EarthwallBehaviour>();
    }

    private void OnEnable()
    {
        counter = 0;
        ResetObject();
    }

    public override void TakeDamage(int amount, System.Type typeOfDamageDealer)
    {
        base.TakeDamage(amount, typeOfDamageDealer);
        if(!IsAlive())
        {
            mEarthWall.EndSpell();
        }
    }

    private void Update()
    {
    }
}
