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

    public delegate void StatsRelevant(int damage, PlayerScript damageDealer, System.Type typeOfDamageDealer, bool lethal);
    public event StatsRelevant OnStatsRelevantDamage;

    public override void TakeDamage(int amount, PlayerScript damageDealer, System.Type typeOfDamageDealer = null) {
        bool hasBeenAlive = IsAlive();
        int actualDamage = mPlayer.effectStateSystem.current.CalculateDamage(amount, typeOfDamageDealer);

        base.TakeDamage(actualDamage, damageDealer, typeOfDamageDealer);

        bool lethalDamage = !IsAlive() && hasBeenAlive;
        if (lethalDamage) {
            //this mPlayer is dead!!! tell the Gamemanager, that one is down
            GameManager.instance.PlayerDown();
            //Show the postGame screen for this player
            RpcPlayerDead();
            NetManager.instance.RpcPlayerDied(damageDealer.playerName, typeOfDamageDealer.AssemblyQualifiedName, mPlayer.playerName);
            mPlayer.SetInputState(InputStateSystem.InputStateID.Dead);
        }

        //fire the stats checks
        if (OnStatsRelevantDamage != null)
        {
            OnStatsRelevantDamage(actualDamage, damageDealer, typeOfDamageDealer, lethalDamage);
        }
    }

    [ClientRpc]
    void RpcPlayerDead()
    {
        //if the player was casting a spell - stop its preview
        mPlayer.GetPlayerSpells().StopPreview();
        mPlayer.inputStateSystem.current.SetPreview(false);

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

    public void TakeFallDamage(int amount)
    {
        TakeDamage(mPlayer.effectStateSystem.current.CalculateFallDamage(amount), mPlayer, typeof(FallDamage));
    }

    public override void HealthChangedHook(int newHealth)
    {
		base.HealthChangedHook(newHealth);
    }

    [Command]
    public void CmdTakeLocalLavaDamage(int amount)
    {
        TakeDamage(amount, mPlayer, typeof(LavaFloor));
    }
}
