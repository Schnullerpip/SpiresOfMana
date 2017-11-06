using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public Text healthText;

    private PlayerHealthScript localPlayerHealthScript;
    private Canvas canvas;

    // Use this for initialization
    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
    }

    public void Init()
    {
        localPlayerHealthScript = GameManager.instance.localPlayer.healthScript;
        // Get notified whenever health is changed
        localPlayerHealthScript.OnHealthChanged += SetHealth;
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