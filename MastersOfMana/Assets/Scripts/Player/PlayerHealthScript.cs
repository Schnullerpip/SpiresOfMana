using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : HealthScript {

    private PlayerScript mPlayer;

    public delegate void DamageTaken(int damage);
    public event DamageTaken OnDamageTaken;

    public delegate void HealthChanged(int damage);
    public event HealthChanged OnHealthChanged;

    public override void Start()
    {
        mPlayer = GetComponent<PlayerScript>();
        base.Start();
    }

    public override void TakeDamage(int amount) {
        bool hasBeenAlive = IsAlive();
        base.TakeDamage(mPlayer.effectStateSystem.current.CalculateDamage(amount));
        if (!IsAlive() && hasBeenAlive) {
            //this mPlayer is dead!!! tell the Gamemanager, that one is down
            GameManager.instance.PlayerDown();
        }
    }

    public override void TakeHeal(int amount)
    {
        base.TakeHeal(mPlayer.effectStateSystem.current.CalculateHeal(amount));
    }

    public void TakeFallDamage(int amount)
    {
        TakeDamage(mPlayer.effectStateSystem.current.CalculateFallDamage(amount));
    }

    public override void HealthChangedHook(int newHealth)
    {
        if (OnHealthChanged != null)
        {
            OnHealthChanged(newHealth);
        }

        // If newHealth is smaller than current Health we have taken damage!
        int damage = GetCurrentHealth() - newHealth;
        if (damage > 0)
        {
            if (OnDamageTaken != null)
            {
                OnDamageTaken(damage);
            }
        }
    }
}
