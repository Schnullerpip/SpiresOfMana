using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MultipleMenuInput : MonoBehaviour {

    private EventSystem eventSystem;
    public GameObject firstSelected;
    private Rewired.ControllerType prevControllerType;
    private Rewired.Player rewiredPlayer;

    public void onControllerChanged(Rewired.Controller controller)
    {
        if(controller.type == Rewired.ControllerType.Mouse && prevControllerType != Rewired.ControllerType.Mouse)
        {
            eventSystem.SetSelectedGameObject(null);
            Cursor.visible = true;
            prevControllerType = controller.type;
        }
        else if(controller.type == Rewired.ControllerType.Joystick && firstSelected != null && prevControllerType != Rewired.ControllerType.Joystick)
        {
            eventSystem.SetSelectedGameObject(firstSelected);
            Cursor.visible = false;
            prevControllerType = controller.type;
        }
    }

    public void Start()
    {
        eventSystem = EventSystem.current;
        rewiredPlayer = Rewired.ReInput.players.GetPlayer(0);
        highlightFirstSelected();
    }

    public void OnEnable()
    {
        Rewired.ActiveControllerChangedDelegate onControllerChangedDelegate = onControllerChanged;
        Rewired.ReInput.controllers.AddLastActiveControllerChangedDelegate(onControllerChangedDelegate);
        highlightFirstSelected();
        GameManager.instance.SetMenuActive(true);
    }

    public void highlightFirstSelected()
    {
		if (firstSelected && eventSystem != null && rewiredPlayer.controllers.GetLastActiveController() != null && rewiredPlayer.controllers.GetLastActiveController().type == Rewired.ControllerType.Joystick)
        {
            eventSystem.SetSelectedGameObject(firstSelected);
        }
    }

    public void OnDisable()
    {
        if (Rewired.ReInput.isReady)
        {
            Rewired.ActiveControllerChangedDelegate onControllerChangedDelegate = onControllerChanged;
            Rewired.ReInput.controllers.RemoveLastActiveControllerChangedDelegate(onControllerChangedDelegate);
        }
        GameManager.instance.SetMenuActive(false);
    }
}
