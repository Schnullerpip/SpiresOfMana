using UnityEngine;
using UnityEngine.UI;
public class PostGameMenu : MonoBehaviour
{
    public Text winnerText;

    public void GoToMainMenu()
    {
        Prototype.NetworkLobby.LobbyManager.s_Singleton.GoBackButton();
    }

    public void Awake()
    {
        winnerText.text = GameManager.instance.winnerName + " has won!"; 
    }
}
