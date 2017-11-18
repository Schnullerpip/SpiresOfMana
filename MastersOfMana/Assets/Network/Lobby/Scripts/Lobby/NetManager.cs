using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetManager : NetworkBehaviour {
    public static NetManager instance;
    public bool initialized = false;

    private void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public bool amIServer()
    {
        return isServer;
    }

    [ClientRpc]
    public void RpcLoadPostGameScreen(uint winner)
    {
        GameManager.instance.winnerID = winner;
        SceneManager.LoadSceneAsync("Scenes/arne_postGame", LoadSceneMode.Additive);
    }

    [ClientRpc]
    public void RpcTriggerGameStarted()
    {
        GameManager.instance.TriggerGameStarted();
    }
}
