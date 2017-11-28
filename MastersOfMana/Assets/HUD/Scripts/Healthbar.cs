using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {

    public Slider healthbar;
    public PlayerScript player;
    private int mPlayerMaxHealth;

    private Vector2 mVelocity;

    public void Init()
    {
        player.healthScript.OnHealthChanged += UpdateHealthbar;
        mPlayerMaxHealth = player.healthScript.GetMaxHealth();
    }

    public void OnDisable()
    {
        player.healthScript.OnHealthChanged -= UpdateHealthbar;
    }

    private void UpdateHealthbar(int newHealth)
    {
        float percentagOfMaxHealth = (float)newHealth / mPlayerMaxHealth;
        healthbar.value = percentagOfMaxHealth;
    }
}
