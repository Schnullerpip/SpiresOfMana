using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : HealthScript {

    private PlayerScript mPlayer;

    public delegate void DamageTaken(float damage);
    public event DamageTaken OnDamageTaken;

    public delegate void HealthChanged(float damage);
    public event HealthChanged OnHealthChanged;

    public override void Start()
    {
        mPlayer = GetComponent<PlayerScript>();
        base.Start();
    }

    public override void TakeDamage(float amount) {
        bool hasBeenAlive = IsAlive();
        base.TakeDamage(mPlayer.effectStateSystem.current.CalculateDamage(amount));

        if (!IsAlive() && hasBeenAlive) {
            //this mPlayer is dead!!! tell the Gamemanager, that one is down
            GameManager.instance.PlayerDown();
        }
    }

    public override void TakeHeal(float amount)
    {
        base.TakeHeal(mPlayer.effectStateSystem.current.CalculateHeal(amount));
    }

    public void TakeFallDamage(float amount)
    {
        TakeDamage(mPlayer.effectStateSystem.current.CalculateFallDamage(amount));
    }

    public override void HealthChangedHook(float newHealth)
    {

        if(OnHealthChanged != null)
        {
            OnHealthChanged(newHealth);
        }
        ////At game start healthHud might not be available yet
        //if (healthHUD)
        //{
        //    healthHUD.SetHealth(newHealth);
        //}

        // If newHealth is smaller than current Health we have taken damage!
        float damage = GetCurrentHealth() - newHealth;
        if (damage > 0)
        {
            if (OnDamageTaken != null)
            {
                OnDamageTaken(damage);
            }
        }
    }
}
