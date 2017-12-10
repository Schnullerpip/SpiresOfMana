using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class IngameLobby : MonoBehaviour {

    protected Rewired.Player mRewiredPlayer;
    private bool mIsMenuActive = false;
    public Canvas canvas;
    public GameObject spellselectionPanel;
    public GameObject lobby;

    void OnEnable()
    {
        GameManager.OnRoundStarted += RoundStarted;
    }

    void RoundStarted()
    {
        gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start ()
    {
        mRewiredPlayer = ReInput.players.GetPlayer(0);
        ToggleVisibility();
    }

    public void ToggleVisibility()
    {
        mIsMenuActive = !mIsMenuActive;
        canvas.enabled = mIsMenuActive;

        Cursor.lockState = mIsMenuActive ? CursorLockMode.None : CursorLockMode.Locked;

        mRewiredPlayer.controllers.maps.SetMapsEnabled(mIsMenuActive, "UI");
        mRewiredPlayer.controllers.maps.SetMapsEnabled(!mIsMenuActive, "Default");

        if (mIsMenuActive)
        {
        }
    }

    public void SpellselectionFinished()
    {
        spellselectionPanel.SetActive(false);
        lobby.gameObject.SetActive(true);
        GameManager.instance.localPlayer.playerLobby.CmdSetReady(true);
    }

    public void OnDisable()
    {
        ToggleVisibility();
        GameManager.OnRoundStarted -= RoundStarted;
    }
}
