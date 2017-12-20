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

    public void Start()
    {
        HUD hud = GetComponentInParent<HUD>();
        resumeButton.onClick.AddListener(hud.ExitPostGameScreen);
    }

    public void OnEnable()
    {
        if (GameManager.instance.winnerID == GameManager.instance.localPlayer.netId.Value)
        {
            text.text = "You won!!!";
        }
        else
        {
            text.text = "You Lost!";
        }
        resumeButton.OnSelect(null);
    }
}
