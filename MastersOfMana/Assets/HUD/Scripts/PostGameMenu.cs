using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PostGameMenu : MonoBehaviour
{
    public Text winnerText;
    public Button resumeButton;

    public void GoToMainMenu()
    {
        Prototype.NetworkLobby.LobbyManager.s_Singleton.GoBackButton();
    }

    public void OnEnable()
    {
        winnerText.text = GameManager.instance.winnerName + " has won!";
        GameObject.FindObjectsOfType<IngameMenu>()[0].mIsMenuActive = true;
        Rewired.ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, "Default");
        Rewired.ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "UI");
        Cursor.lockState =  CursorLockMode.None;
        resumeButton.OnSelect(null);
    }
}
