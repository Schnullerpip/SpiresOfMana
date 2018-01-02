using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public Text healthText;
    public OpponentHUD opponentHUDPrefab;

    private PlayerHealthScript localPlayerHealthScript;
    private bool isInitialized = false;

    public Transform healthScale;
    public Gradient healthGradient;
    public Image filling;
    public float scrollingSpeed = 2;

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

        //when enabled, set the hud to the maximum instantly, this should avoid the bar filling on a restart from 0
        SetHealthInstant(localPlayerHealthScript.GetMaxHealth());
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

        mCurrentHealth = localPlayerHealthScript.GetMaxHealth();
    }

    void OnDisable()
    {
        GameManager.OnLocalPlayerDead -= LocalPlayerDead;
        if (localPlayerHealthScript)
        {
            localPlayerHealthScript.OnHealthChanged -= SetHealth;
        }
    }

    private int mCurrentHealth;

    /// <summary>
    /// Sets the health in the HUD over time.
    /// </summary>
    /// <param name="health">Health.</param>
    public void SetHealth(int health)
    {
        StopAllCoroutines();
        StartCoroutine(HealthScroll(health, scrollingSpeed));
    }

    /// <summary>
    /// Sets the health in the HUD instantly.
    /// </summary>
    /// <param name="health">Health.</param>
    public void SetHealthInstant(int health)
    {
        StopAllCoroutines();
        mCurrentHealth = health;
        healthText.text = mCurrentHealth.ToString();
		ScaleAndColor();
    }

    private IEnumerator HealthScroll(int newHealth, float speed)
    {
        //convert the health to a float, so we can calculate sub-integer
        float currentHealthFloat = mCurrentHealth;

        while(Mathf.Abs(currentHealthFloat - newHealth) >= float.Epsilon)
        {
            currentHealthFloat = Mathf.MoveTowards(currentHealthFloat, newHealth, Time.deltaTime * speed);
            ScaleAndColor();

            //this part is important, as this allows us to just kill the coroutine and start a new one
            mCurrentHealth = Mathf.RoundToInt(currentHealthFloat);
            healthText.text = mCurrentHealth.ToString();

            yield return null;
        }

        //when done, adjust the color, scale and number once more to make sure that we didn't overshoot
        ScaleAndColor();

        mCurrentHealth = newHealth;
        healthText.text = mCurrentHealth.ToString();
    }

    private void ScaleAndColor()
    {
        float f = mCurrentHealth / 100.0f;
        healthScale.localScale = healthScale.localScale.Change(y: f);
        filling.color = healthGradient.Evaluate(1 - f);
    }

	private void LocalPlayerDead()
	{
        gameObject.SetActive(false);
    }
}