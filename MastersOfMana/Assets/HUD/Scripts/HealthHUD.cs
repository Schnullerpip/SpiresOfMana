using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public Text healthText;
    public float criticalHealthFraction;

    private PlayerHealthScript localPlayerHealthScript;
    private Canvas canvas;
    private float mCriticalHealth;
    private bool mInCritialHealth = false;

    // Use this for initialization
    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
    }

    public void Init()
    {
        localPlayerHealthScript = GameManager.instance.localPlayer.healthScript;
        // Set this UI Script in HealthScript so that HealtScript can update us
        localPlayerHealthScript.healthHUD = this;
        mCriticalHealth = criticalHealthFraction * localPlayerHealthScript.GetMaxHealth();
        SetHealth(localPlayerHealthScript.GetCurrentHealth());
        canvas = GetComponentInParent<Canvas>();
        canvas.enabled = true;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
    }

    public void SetHealth(float health)
    {
        healthText.text = "Health: " + (int)health;
    }
}