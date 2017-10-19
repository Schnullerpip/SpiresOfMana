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

    [ClientRpc]
    public void RpcLoadPostGameScreen(string winner)
    {
        GameManager.instance.winnerName = winner;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }

    [ClientRpc]
    public void RpcTriggerGameStarted()
    {
        GameManager.instance.TriggerGameStarted();
    }
}
