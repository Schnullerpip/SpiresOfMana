using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealthScript : HealthScript {

	public PitchingAudioClip[] damageSFXs;
	private AudioSource mSFXaudioSource;

    private PlayerScript mPlayer;

    public delegate void DamageTaken(int damage);
    public event DamageTaken OnDamageTaken;

    public delegate void HealTaken(int damage);
    public event HealTaken OnHealTaken;

    public delegate void HealthChanged(int damage);
    public event HealthChanged OnHealthChanged;

    public override void Start()
    {
        mPlayer = GetComponent<PlayerScript>();
        base.Start();
		mSFXaudioSource = GetComponent<AudioSource>();
    }

    public delegate void StatsRelevant(int damage, System.Type typeOfDamageDealer);
    public event StatsRelevant OnStatsRelevantDamage;

    public override void TakeDamage(int amount, System.Type typeOfDamageDealer) {
        bool hasBeenAlive = IsAlive();
        int actualDamage = mPlayer.effectStateSystem.current.CalculateDamage(amount);
        base.TakeDamage(actualDamage, typeOfDamageDealer);
        if (!IsAlive() && hasBeenAlive) {
            //this mPlayer is dead!!! tell the Gamemanager, that one is down
            GameManager.instance.PlayerDown();
            //Show the postGame screen for this player
            RpcPlayerDead();
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

        if (OnHealthChanged != null)
        {
            OnHealthChanged(newHealth);
        }

        // If newHealth is smaller than current Health we have taken damage!
        int damage = GetCurrentHealth() - newHealth;
        if (damage > 0)
        {
			PlayRandomHurtSFX();
            if (OnDamageTaken != null)
            {
                OnDamageTaken(damage);
            }
        }
        else if(damage < 0)
        {
            if(OnHealTaken != null)
            {
                OnHealTaken(Mathf.Abs(damage));
            }
        }
    }

	public void PlayRandomHurtSFX()
	{
		PitchingAudioClip clip = damageSFXs.RandomElement();
		mSFXaudioSource.pitch = clip.GetRandomPitch();
		mSFXaudioSource.PlayOneShot(clip.audioClip);
	}
}
