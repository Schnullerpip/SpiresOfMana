using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class CustomNetworkDiscovery : UnityEngine.Networking.NetworkDiscovery {

	public Prototype.NetworkLobby.LobbyServerList lobbyServerList;
	public Prototype.NetworkLobby.LobbyManager lobbyManager;
	public List<Prototype.NetworkLobby.LobbyServerEntry> localMatches = new List<Prototype.NetworkLobby.LobbyServerEntry> ();
	public Prototype.NetworkLobby.LobbyServerEntry serverEntryPrefab;
    private Dictionary<string, Coroutine> mAddressMatchDicitonary = new Dictionary<string, Coroutine>();

    public bool CustomStartAsServer()
    {
        if (!running)
        {
            //NetworkTransport.Shutdown();
            //NetworkTransport.Init();
            Initialize();
        }
        //if (!isServer)
        return StartAsServer();
    }

    public bool CustomStartAsClient()
    {
        NetworkTransport.Shutdown();
        NetworkTransport.Init();
        return StartAsClient();
    }

    static string BytesToString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    private IEnumerator RemoveBroadcastDataDelayed(string address)
    {
        yield return new WaitForSeconds(broadcastInterval * 0.002f);
        if (mAddressMatchDicitonary.ContainsKey(address))
        {
            StopCoroutine(mAddressMatchDicitonary[address]);
            mAddressMatchDicitonary.Remove(address);
            if (broadcastsReceived != null && broadcastsReceived.ContainsKey(address))
            {
                broadcastsReceived.Remove(address);
            }
        }
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        if(mAddressMatchDicitonary.ContainsKey(fromAddress))
        {
            StopCoroutine(mAddressMatchDicitonary[fromAddress]);
            mAddressMatchDicitonary.Remove(fromAddress);
        }
        mAddressMatchDicitonary.Add(fromAddress, StartCoroutine(RemoveBroadcastDataDelayed(fromAddress)));
    }

    public void UpdateLocalMatches()
    {
        ClearMatchList();

        if (broadcastsReceived != null)
        {
            foreach (var addr in broadcastsReceived.Keys)
            {
                var value = broadcastsReceived[addr];
                string dataString = BytesToString(value.broadcastData);
                var items = dataString.Split(':');
                Prototype.NetworkLobby.LobbyServerEntry obj = Instantiate(serverEntryPrefab);
                obj.setupLocalMatch("Local: " + items[1], () => { startCallback(items[1]); });
                localMatches.Add(obj);
            }
        }
    }

    private void ClearMatchList()
    {
        foreach (Prototype.NetworkLobby.LobbyServerEntry entry in localMatches)
        {
            if (entry)
            {
                Destroy(entry.gameObject);
            }
        }
        localMatches.Clear();
    }

    void OnDisable()
    {
        ClearMatchList();
    }

    public void SafeStopBroadcast()
    {
        if (hostId != -1 && running)
        {
            StopBroadcast();
        }
    }
	
	public void startCallback(string data)
	{
        lobbyManager.DisplayIsConnecting();
        lobbyManager.networkAddress = data;
        SafeStopBroadcast();
        lobbyManager.StartClient ();
	}
}

