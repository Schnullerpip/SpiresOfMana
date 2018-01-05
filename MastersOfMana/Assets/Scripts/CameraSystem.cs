using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour {

    private GameObject mcurrentActiveCamera;

    public enum Cameras
    {
        SpectatorCamera,
        PlayerCamera,
        CinematicCamera
    }
    private Dictionary<Cameras, GameObject> registeredCameraObjects= new Dictionary<Cameras, GameObject>();

    public void RegisterCameraObject(Cameras cameraName, GameObject cameraObject)
    {
        if(registeredCameraObjects.ContainsKey(cameraName))
        {
            registeredCameraObjects.Remove(cameraName);
        }
        registeredCameraObjects.Add(cameraName, cameraObject);
    }

    public void ActivateCamera(Cameras cameraName)
    {
        if (!mcurrentActiveCamera)
        {
            mcurrentActiveCamera = Camera.main.gameObject;
        }
        mcurrentActiveCamera.SetActive(false);
        GameObject newActiveCamera = registeredCameraObjects[cameraName];
        newActiveCamera.SetActive(true);
        mcurrentActiveCamera = newActiveCamera;
    }

    public GameObject GetCameraObject(Cameras cameraName)
    {
        return registeredCameraObjects[cameraName];
    }

}
