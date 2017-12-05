using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public Text healthText;
    public OpponentHUD opponentHUDPrefab;

    private PlayerHealthScript localPlayerHealthScript;
    private Canvas canvas;

    // Use this for initialization
    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
		GameManager.OnLocalPlayerDead += LocalPlayerDead;
    }

    public void Init()
    {
        GameManager gameManager = GameManager.instance;
        localPlayerHealthScript = gameManager.localPlayer.healthScript;
        // Get notified whenever health is changed
        localPlayerHealthScript.OnHealthChanged += SetHealth;
        SetHealth(localPlayerHealthScript.GetCurrentHealth());
        canvas = GetComponentInParent<Canvas>();
        canvas.enabled = true;

        //Create one floating damge text system per non-local player so we can cache the transform
        foreach(PlayerScript player in FindObjectsOfType<PlayerScript>())
        {
            //Don't create a system for the local player
            if (player.netId == gameManager.localPlayer.netId)
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
        GameManager.OnGameStarted -= Init;
        GameManager.OnLocalPlayerDead -= LocalPlayerDead;
    }

    public void SetHealth(int health)
    {
        healthText.text = "Health: " + health;
    }

	private void LocalPlayerDead()
	{
        Debug.Log(canvas);
        if(!canvas)
        {
            canvas = GetComponentInParent<Canvas>();
        }
        canvas.enabled = false;
	}
}