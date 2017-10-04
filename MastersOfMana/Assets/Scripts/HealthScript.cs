using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script that provides an interface to anything, that can be damaged 
/// a currentHealth field is supposed to keep track of the GameObject's current health,
/// while maxHealth is the GameObject's initial health value
/// </summary>
public class HealthScript : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100, currentHealth;

    //states whether the GameObject is alive or not
    private bool isAlive = true;
    public bool IsAlive() {
        return isAlive;
    }

    // Use this for initialization
    void Start()
    {
        Reset();
    }

    //public interface

    //the only thing, that should be adressed, to actually hurt a GameObject
    public virtual void TakeDamage(float amount) {
        if ((currentHealth -= amount) <= 0) {
            currentHealth = 0;
            isAlive = false;
        }
    }

    //bring a GameObject to life
    public void Reset()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    public float GetCurrentHealth() {
        return currentHealth;
    }
}