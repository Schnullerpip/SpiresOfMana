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

    private void Awake()
    {
        GameObject cinematicCamera = Instantiate(cineCamPrefab);
        GameManager.instance.cameraSystem.RegisterCameraObject(CameraSystem.Cameras.CinematicCamera, cinematicCamera.gameObject);
        GameManager.OnRoundStarted += RoundStarted;
    }   

    public void OnEnable()
    {
        spellselectionPanel.SetActive(true);
        lobby.gameObject.SetActive(false);
        GameManager.instance.cameraSystem.ActivateCamera(CameraSystem.Cameras.CinematicCamera);
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

    public void OnDestroy()
    {
        GameManager.OnRoundStarted -= RoundStarted;
    }
}
