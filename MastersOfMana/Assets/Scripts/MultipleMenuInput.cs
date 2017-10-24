using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MultipleMenuInput : MonoBehaviour {

    private EventSystem eventSystem;
    public GameObject firstSelected;

    public void onControllerChanged(Rewired.Controller controller)
    {
        if(controller.type == Rewired.ControllerType.Mouse)
        {
            eventSystem.SetSelectedGameObject(null);
        }
        else if(controller.type == Rewired.ControllerType.Joystick)
        {
            eventSystem.SetSelectedGameObject(firstSelected);
        }
    }

    public void Start()
    {
        eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void OnEnable()
    {
        Rewired.ActiveControllerChangedDelegate onControllerChangedDelegate = onControllerChanged;
        Rewired.ReInput.controllers.AddLastActiveControllerChangedDelegate(onControllerChangedDelegate);
    }

    public void OnDisable()
    {
        if (Rewired.ReInput.isReady)
        {
            Rewired.ActiveControllerChangedDelegate onControllerChangedDelegate = onControllerChanged;
            Rewired.ReInput.controllers.RemoveLastActiveControllerChangedDelegate(onControllerChangedDelegate);
        }
    }
}
