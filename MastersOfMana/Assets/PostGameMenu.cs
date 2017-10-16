using UnityEngine;

public class PostGameMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Prototype.NetworkLobby.LobbyManager.s_Singleton.GoBackButton();
    }
}
