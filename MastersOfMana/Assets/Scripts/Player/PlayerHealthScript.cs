using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealthScript : HealthScript 
{
	
    private PlayerScript mPlayer;

    public override void Start()
    {
        mPlayer = GetComponent<PlayerScript>();
        base.Start();
    }

    public delegate void StatsRelevant(int damage, System.Type typeOfDamageDealer);
    public event StatsRelevant OnStatsRelevantDamage;

    public override void TakeDamage(int amount, System.Type typeOfDamageDealer) {
        bool hasBeenAlive = IsAlive();
        int actualDamage = mPlayer.effectStateSystem.current.CalculateDamage(amount, typeOfDamageDealer);
        base.TakeDamage(actualDamage, typeOfDamageDealer);
        if (!IsAlive() && hasBeenAlive) {
            //this mPlayer is dead!!! tell the Gamemanager, that one is down
            GameManager.instance.PlayerDown();
            //Show the postGame screen for this player
            RpcPlayerDead();
            mPlayer.SetInputState(InputStateSystem.InputStateID.Dead);
        }

        //fire the stats checks
        if (OnStatsRelevantDamage != null)
        {
            OnStatsRelevantDamage(actualDamage, typeOfDamageDealer);
        }
    }

    [ClientRpc]
    void RpcPlayerDead()
    {
        if(isLocalPlayer)
        {
            GameManager.instance.localPlayerDead();
        }
    }

    public override void TakeHeal(int amount)
    {
        base.TakeHeal(mPlayer.effectStateSystem.current.CalculateHeal(amount));
    }

    public struct FallDamage { };
    public static FallDamage fallDamageInstance;

    public void TakeFallDamage(int amount)
    {
        TakeDamage(mPlayer.effectStateSystem.current.CalculateFallDamage(amount), fallDamageInstance.GetType());
    }

    public override void HealthChangedHook(int newHealth)
    {
		base.HealthChangedHook(newHealth);
    }
}
