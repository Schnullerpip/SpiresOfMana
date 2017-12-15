using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public Text healthText;
    public OpponentHUD opponentHUDPrefab;

    private PlayerHealthScript localPlayerHealthScript;
    private bool isInitialized = false;

    // Use this for initialization
    void OnEnable()
    {
		GameManager.OnLocalPlayerDead += LocalPlayerDead;
        if(!isInitialized)
        {
            Init();
        }
        // Get notified whenever health is changed
        localPlayerHealthScript.OnHealthChanged += SetHealth;
        SetHealth(localPlayerHealthScript.GetCurrentHealth());
    }

    public void Init()
    {
        isInitialized = true;
        localPlayerHealthScript = GameManager.instance.localPlayer.healthScript;
        //Create one floating damge text system per non-local player so we can cache the transform
        foreach (PlayerScript player in FindObjectsOfType<PlayerScript>())
        {
            //Don't create a system for the local player
            if (player.netId == GameManager.instance.localPlayer.netId)
            {
                continue;
            }

            OpponentHUD oponnentHUD = Instantiate(opponentHUDPrefab);
            oponnentHUD.player = player;
            oponnentHUD.Init();
        }
    }

    void OnDisable()
    {
        GameManager.OnLocalPlayerDead -= LocalPlayerDead;
        if (localPlayerHealthScript)
        {
            localPlayerHealthScript.OnHealthChanged -= SetHealth;
        }
    }

    public void SetHealth(int health)
    {
        healthText.text = "Health: " + health;
    }

	private void LocalPlayerDead()
	{
        gameObject.SetActive(false);
    }
}