using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class IngameLobby : MonoBehaviour 
{
    public Canvas canvas;
    public GameObject spellselectionPanel;
    public GameObject lobby;
    public GameObject cineCamPrefab;

    private GameObject mCineCam;

    private void Awake()
    {
        mCineCam = Instantiate(cineCamPrefab);
    }

    void OnEnable()
    {
        GameManager.OnRoundStarted += RoundStarted;
        spellselectionPanel.SetActive(true);
        lobby.gameObject.SetActive(false);
        mCineCam.SetActive(true);
    }

    void RoundStarted()
    {
        gameObject.SetActive(false);
    }

    public void SpellselectionFinished()
    {
		spellselectionPanel.GetComponent<SpellSelectionPanel> ().OnReady ();
        spellselectionPanel.SetActive(false);
        lobby.gameObject.SetActive(true);
        GameManager.instance.localPlayer.playerLobby.CmdSetReady(true);
    }

    public void OnDisable()
    {
        GameManager.OnRoundStarted -= RoundStarted;
        if (mCineCam)
        {
            mCineCam.SetActive(false);
        }
    }
}
