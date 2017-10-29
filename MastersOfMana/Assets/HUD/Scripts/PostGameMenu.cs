using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PostGameMenu : MonoBehaviour
{
    public Text winnerText;

    public void GoToMainMenu()
    {
        Prototype.NetworkLobby.LobbyManager.s_Singleton.GoBackButton();
    }

    public void Awake()
    {
        GameObject.FindObjectsOfType<IngameMenu>()[0].mIsMenuActive = true;
        winnerText.text = GameManager.instance.winnerName + " has won!";
        Cursor.lockState =  CursorLockMode.None;
        ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "UI");
    }
}
