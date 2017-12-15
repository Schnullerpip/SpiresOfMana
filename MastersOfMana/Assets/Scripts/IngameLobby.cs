using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class IngameLobby : MonoBehaviour {
    public Canvas canvas;
    public GameObject spellselectionPanel;
    public GameObject lobby;
	public Camera cam;

    void OnEnable()
    {
        GameManager.OnRoundStarted += RoundStarted;
        GameManager.instance.numOfActiveMenus++;
        GameManager.instance.OnApplicationFocus(true);
        spellselectionPanel.SetActive(true);
        lobby.gameObject.SetActive(false);
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
        GameManager.instance.numOfActiveMenus--;
        GameManager.instance.OnApplicationFocus(true);
        GameManager.OnRoundStarted -= RoundStarted;
    }
}
