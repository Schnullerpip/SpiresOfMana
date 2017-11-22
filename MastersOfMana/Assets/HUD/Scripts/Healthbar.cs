using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {

    public Slider healthbar;
    public PlayerScript player;
    private int mPlayerMaxHealth;
    private Camera mCamera;

    public float maxDistance = 80;
    public float maxAdditionalScale = 2;
    private float mMaxDistanceSqrt;

    private Vector2 mVelocity;

    public void Init()
    {
        mCamera = Camera.main;
        player.healthScript.OnHealthChanged += UpdateHealthbar;
        mPlayerMaxHealth = player.healthScript.GetMaxHealth();
        mMaxDistanceSqrt = maxDistance * maxDistance;
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

    public void Update()
    {
        //Billboard to player
        Vector3 v = mCamera.transform.position - transform.position;
        float scaleFactor = Mathf.Clamp01(v.sqrMagnitude / mMaxDistanceSqrt) * maxAdditionalScale + 1;
        healthbar.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        v.x = v.z = 0.0f;
        healthbar.transform.LookAt(mCamera.transform.position - v);
        healthbar.transform.Rotate(0, 180, 0);
    }
}
