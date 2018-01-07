using UnityEngine;
using Rewired;

public class IngameMenu : MonoBehaviour
{
    public Prototype.NetworkLobby.LobbyManager lobbyManager;
    public Canvas canvas;
    protected Rewired.Player mRewiredPlayer;
    public MultipleMenuInput menuInput;
    public RectTransform confirmationPanel;
    public RectTransform optionsPanel;

    public UnityEngine.UI.Selectable defaultSelected;

    private void Awake()
    {
        optionsPanel.GetComponent<Prototype.NetworkLobby.LobbyOptions>().ApplyAudioSettings();
    }

    public void ToggleVisibility()
    {
        canvas.enabled = !canvas.enabled;
        //Small hack to use the OnDisable/OnEnable functionality of the MultipleMenuInput script
        menuInput.enabled = canvas.enabled;

        if (GameManager.instance.numOfActiveMenus > 0 && lobbyManager)
        {
            if (defaultSelected != null)
            {
                defaultSelected.Select();
            }
            lobbyManager.SetCancelDelegate(null);
        }
    }

    public void Resume()
    {
        if (lobbyManager)
        {
            lobbyManager.RemoveLastCancelDelegate();
        }
        confirmationPanel.gameObject.SetActive(false);
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
        if (NetManager.instance.amIServer())
        {
            lobbyManager.StopHostClbk();
        }
        else
        {
            lobbyManager.StopClientClbk();
        }
    }

    public void GoToOptionsMenu()
    {
        optionsPanel.gameObject.SetActive(true);
    }

    public void OpenConfirmationPanel()
    {
        confirmationPanel.gameObject.SetActive(true);
    }

    //This makes sure that we set GameRunning to false in any case where we change the game scene
    private void OnDestroy()
    {
        GameManager.instance.gameRunning = false;
    }
}
