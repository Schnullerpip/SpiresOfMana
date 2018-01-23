using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class IngameLobby : MonoBehaviour 
{
    public GameObject cineCamPrefab;

    private void Awake()
    {
        GameObject cinematicCamera = Instantiate(cineCamPrefab);
        GameManager.instance.cameraSystem.RegisterCameraObject(CameraSystem.Cameras.CinematicCamera, cinematicCamera.gameObject);
        GameManager.OnRoundStarted += RoundStarted;
    }   

    public void OnEnable()
    {
        GameManager.instance.cameraSystem.ActivateCamera(CameraSystem.Cameras.CinematicCamera);
        GameManager.instance.localPlayer.SetInputState(InputStateSystem.InputStateID.Lobby);
    }

    void RoundStarted()
    {
        gameObject.SetActive(false);
        GameManager.instance.localPlayer.SetInputState(InputStateSystem.InputStateID.Normal);
    }

    public void OnDestroy()
    {
        GameManager.OnRoundStarted -= RoundStarted;
    }
}
