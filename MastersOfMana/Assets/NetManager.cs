using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetManager : NetworkBehaviour {
    public static NetManager instance;

    private void Start()
    {
        if(instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    [ClientRpc]
    public void RpcLoadPostGameScreen(string winner)
    {
        GameManager.instance.winnerName = winner;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}
