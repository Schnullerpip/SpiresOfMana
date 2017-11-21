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
        if (isClient)
        {
            //GameManager on client doe not have the players yet - get them
            var players = FindObjectsOfType<PlayerScript>();
            foreach (var p in players)
            {
                GameManager.instance.mPlayers.Add(p);
            }
        }

        GameManager.instance.TriggerGameStarted();
    }
}
