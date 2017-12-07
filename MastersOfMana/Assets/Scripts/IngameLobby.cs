using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class IngameLobby : MonoBehaviour {

    protected Rewired.Player mRewiredPlayer;
    private bool mIsMenuActive = false;
    public Canvas canvas;
    public Prototype.NetworkLobby.SpellSelectionPanel spellselectionPanel;

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
        spellselectionPanel.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        ToggleVisibility();
    }
}
