using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script that provides an interface to anything, that can be damaged 
/// a mCurrentHealth field is supposed to keep track of the GameObject's current health,
/// while mMaxHealth is the GameObject's initial health value
/// </summary>
public class HealthScript : MonoBehaviour
{
    [SerializeField]
    private float mMaxHealth = 100, mCurrentHealth;

    //states whether the GameObject is alive or not
    private bool mIsAlive = true;
    public bool IsAlive() {
        return mIsAlive;
    }

    // Use this for initialization
    void Start()
    {
        Reset();
    }

    //public interface

    //the only thing, that should be adressed, to actually hurt a GameObject
    public virtual void TakeDamage(float amount) {
        if ((mCurrentHealth -= amount) <= 0) {
            mCurrentHealth = 0;
            mIsAlive = false;
        }
    }

    //bring a GameObject to life
    public void Reset()
    {
        mCurrentHealth = mMaxHealth;
        mIsAlive = true;
    }
}