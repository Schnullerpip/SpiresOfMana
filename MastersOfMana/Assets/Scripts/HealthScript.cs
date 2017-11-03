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
    private float mMaxHealth = 50;
    [SerializeField][SyncVar(hook = "HealthHook")]
    private float mCurrentHealth;

    //states whether the GameObject is alive or not
    private bool isAlive = true;
    public bool IsAlive() {
        return isAlive;
    }

    // Use this for initialization
    public virtual void Start()
    {
        Reset();
    }

    //public interface
    /// <summary>
    /// Calls whenever the health is changed
    /// </summary>
    public void HealthHook(float newHealth)
    {
        OnHealthChanged(newHealth);
    }
    public virtual void OnHealthChanged(float newHealth) {}
        
    /// <summary>
    /// the only thing, that should be adressed, to actually hurt a GameObject, this should only ever be run on the server!!!
    /// </summary>
    /// <param name="amount"></param>
    public virtual void TakeDamage(float amount) {
        if ((mCurrentHealth -= amount) <= 0) {
            mCurrentHealth = 0;
            isAlive = false;
        }
    }
    
    //bring a GameObject to life
    public void Reset()
    {
        mCurrentHealth = mMaxHealth;
        isAlive = true;
    }

    public float GetCurrentHealth() {
        return mCurrentHealth;
    }

    public float GetMaxHealth()
    {
        return mMaxHealth;
    }
}