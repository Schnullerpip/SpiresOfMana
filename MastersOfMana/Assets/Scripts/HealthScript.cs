using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// The script that provides an interface to anything, that can be damaged 
/// a mCurrentHealth field is supposed to keep track of the GameObject's current health,
/// while mMaxHealth is the GameObject's initial health value
/// </summary>
public class HealthScript : NetworkBehaviour
{
    [SerializeField][SyncVar]
    private int mMaxHealth = 50;
    [SerializeField][SyncVar(hook = "HealthHook")]
    private int mCurrentHealth;

	[Header("SFX")]
	public PitchingAudioClip[] damageSFXs;
	public AudioClip deathSFX;
	public AudioSource SFXaudioSource;

	public delegate void DamageTaken(int damage);
	public event DamageTaken OnDamageTaken;

	public delegate void HealTaken(int damage);
	public event HealTaken OnHealTaken;

	public delegate void HealthChanged(int damage);
	public event HealthChanged OnHealthChanged;

    public delegate void Died();
    public event Died OnInstanceDied;

    //states whether the GameObject is alive or not
    [SyncVar]
    private bool isAlive = true;
    public bool IsAlive() {
        return isAlive;
    }

    // Use this for initialization
    public virtual void Start()
    {
        ResetObject();
        GameManager.OnRoundStarted += ResetObject;
    }

    //public interface
    /// <summary>
    /// Calls whenever the health is changed
    /// </summary>
    public void HealthHook(int newHealth)
    {
        if(newHealth == 0)
        {
            if(isAlive)
            {
                isAlive = false;
                PlayDeathSFX();
			}
        }
        HealthChangedHook(newHealth);
        // we need to set this value manually, because we have a hook attached 
        mCurrentHealth = newHealth;
    }
    public virtual void HealthChangedHook(int newHealth) 
	{
		if (OnHealthChanged != null)
		{
			OnHealthChanged(newHealth);
		}

		// If newHealth is smaller than current Health we have taken damage!
		int damage = GetCurrentHealth() - newHealth;
		if (damage > 0)
		{
			if(newHealth != 0)
			{
				PlayRandomHurtSFX();
			}
				
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

    /// <summary>
    /// the only thing, that should be adressed, to actually hurt a GameObject, this should only ever be run on the server!!!
    /// </summary>
    /// <param name="amount"></param>
    public virtual void TakeDamage(int amount, System.Type typeOfDamageDealer) {
        // The temp variable is used because otherwise the syncvar hook will fire with a negative value if we do the if((mCurrentHealth -= amoun) <= 0) comparison 
        int tempCurrentHealth = mCurrentHealth - amount;
        if (tempCurrentHealth <= 0) {
            mCurrentHealth = 0;
            if (OnInstanceDied != null)
            {
                OnInstanceDied();
            }
        }
        else
        {
            mCurrentHealth = tempCurrentHealth;
        }
    }
    
    //bring a GameObject to life
    public void ResetObject()
    {
        mCurrentHealth = mMaxHealth;
        isAlive = true;
    }

    public int GetCurrentHealth() {
        return mCurrentHealth;
    }

    public int GetMaxHealth()
    {
        return mMaxHealth;
    }

    /// <summary>
    /// the only thing, that should be adressed, to actually heal a GameObject, this should only ever be run on the server!!!
    /// </summary>
    /// <param name="amount"></param>
    public virtual void TakeHeal(int amount)
    {
        if(mCurrentHealth <= 0)
        {
            return;
        }

        //Setting mCurrenthealth here directly would provocate a health changed hook
        int tempHealth = mCurrentHealth + amount;
        if(tempHealth > mMaxHealth)
        {
            mCurrentHealth = mMaxHealth;
        }
        else
        {
            mCurrentHealth = tempHealth;
        }
    }

	public void PlayRandomHurtSFX()
	{
		if(damageSFXs.Length == 0)
		{
			//no sounds
			return;
		}
		PitchingAudioClip clip = damageSFXs.RandomElement();
		SFXaudioSource.pitch = clip.GetRandomPitch();
		SFXaudioSource.PlayOneShot(clip.audioClip);
	}

	public void PlayDeathSFX()
	{
		if(deathSFX == null)
		{
			return;
		}

		SFXaudioSource.pitch = 1;
		SFXaudioSource.PlayOneShot(deathSFX);
	}

    void OnDisable()
    {
        GameManager.OnRoundStarted -= ResetObject;
    }
}