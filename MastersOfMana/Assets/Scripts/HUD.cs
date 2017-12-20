using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

    public List<GameObject> HudPrefabs = new List<GameObject>();
    private SpellHUD mSpellHUD;
    private PostGameMenu mPostGameScreen;
    private IngameLobby mLobby;
    private HealthHUD mHealthHUD;
    private HurtIndicator mHurtIndicator;

    void OnEnable()
    {
        GameManager.OnGameStarted += Init;
        GameManager.OnRoundStarted += RoundStarted;
    }

	// Use this for initialization
	void Init()
    {
        foreach(GameObject obj in HudPrefabs)
        {
            Instantiate(obj, transform);
        }
        mSpellHUD = GetComponentInChildren<SpellHUD>();

        mPostGameScreen = GetComponentInChildren<PostGameMenu>();
        mPostGameScreen.gameObject.SetActive(false);

        mLobby = GetComponentInChildren<IngameLobby>();

        mHealthHUD = GetComponentInChildren<HealthHUD>();
        mHealthHUD.gameObject.SetActive(false);

        mHurtIndicator = GetComponentInChildren<HurtIndicator>();
        mHurtIndicator.gameObject.SetActive(false);
    }

    public SpellHUD GetSpellHUD()
    {
        return mSpellHUD;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= Init;
    }

   public void ShowPostGameScreen(bool show)
    {
        mPostGameScreen.gameObject.SetActive(show);
    }

    void RoundStarted()
    {
        mHealthHUD.gameObject.SetActive(true);
    }

    public void ExitPostGameScreen()
    {
        ShowPostGameScreen(false);
        mLobby.gameObject.SetActive(true);
        mSpellHUD.gameObject.SetActive(true);
        GameManager.instance.TriggerRoundEnded();
    }
}
