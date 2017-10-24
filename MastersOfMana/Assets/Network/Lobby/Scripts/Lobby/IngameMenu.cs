using UnityEngine;
using Rewired;

public class IngameMenu : MonoBehaviour {

    public Prototype.NetworkLobby.LobbyManager lobbyManager;
    public Canvas canvas;
    protected Rewired.Player mRewiredPlayer;

	public UnityEngine.UI.Selectable defaultSelected;

	private bool mIsMenuActive = false;

	public bool GetIsMenuActive()
	{
		return mIsMenuActive;
	}

    public void ToggleVisibility()
    {
		mIsMenuActive = !mIsMenuActive;
		canvas.enabled = mIsMenuActive;

		Cursor.lockState = mIsMenuActive ? CursorLockMode.None : CursorLockMode.Locked;

		mRewiredPlayer.controllers.maps.SetMapsEnabled(mIsMenuActive,"UI");
		mRewiredPlayer.controllers.maps.SetMapsEnabled(!mIsMenuActive,"Default");

		if(mIsMenuActive)
		{
			if(defaultSelected != null)
			{
				defaultSelected.Select();
			}
		}
    }

    public void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            Cursor.lockState = mIsMenuActive ? CursorLockMode.None : CursorLockMode.Locked;
        }
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
            ToggleVisibility();
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
        ToggleVisibility();
    }
}
