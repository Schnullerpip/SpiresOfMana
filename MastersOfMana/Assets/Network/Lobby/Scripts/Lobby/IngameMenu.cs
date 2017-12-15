using UnityEngine;
using Rewired;

public class IngameMenu : MonoBehaviour {

    public Prototype.NetworkLobby.LobbyManager lobbyManager;
    public Canvas canvas;
    protected Rewired.Player mRewiredPlayer;

	public UnityEngine.UI.Selectable defaultSelected;

    public void ToggleVisibility()
    {
		canvas.enabled = !canvas.enabled;

		if(GameManager.instance.numOfActiveMenus > 0 && lobbyManager)
		{
			if(defaultSelected != null)
			{
				defaultSelected.Select();
			}
            lobbyManager.SetCancelDelegate(null);
		}
    }

    public void Resume()
    {
        if(lobbyManager)
        {
            lobbyManager.RemoveLastCancelDelegate();
        }
        ToggleVisibility();
    }

    void OnEnable()
    {
        //initialize Inpur handler
        mRewiredPlayer = ReInput.players.GetPlayer(0);
        lobbyManager = GameObject.FindObjectOfType<Prototype.NetworkLobby.LobbyManager>();
    }

    void Update()
    {
        if (mRewiredPlayer != null && mRewiredPlayer.GetButtonDown("IngameMenu"))
        {
            Resume();
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }

    public void GoToMainMenu()
    {
        lobbyManager.GoBackButton();
        Resume();
    }
}
