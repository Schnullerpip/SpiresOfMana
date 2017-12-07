using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PostGameMenu : MonoBehaviour
{
    public Text text;
    public Button resumeButton;

    public void GoToMainMenu()
    {
        Prototype.NetworkLobby.LobbyManager.s_Singleton.GoBackButton();
    }

    public void OnEnable()
    {
        if(GameManager.instance.winnerID == GameManager.instance.localPlayer.netId.Value)
        {
            text.text = "You won!!!";
        }
        else
        {
            text.text = "You Lost!";
        }
        GameManager.instance.numOfActiveMenus++;
        Rewired.ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, "Default");
        Rewired.ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "UI");
        Cursor.lockState =  CursorLockMode.None;
        resumeButton.OnSelect(null);
    }
}
