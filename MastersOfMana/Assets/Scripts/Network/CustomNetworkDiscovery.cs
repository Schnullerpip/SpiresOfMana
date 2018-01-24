using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class CustomNetworkDiscovery : UnityEngine.Networking.NetworkDiscovery {

	public Prototype.NetworkLobby.LobbyServerList lobbyServerList;
	public Prototype.NetworkLobby.LobbyManager lobbyManager;
	public List<Prototype.NetworkLobby.LobbyServerEntry> localMatches = new List<Prototype.NetworkLobby.LobbyServerEntry> ();
	private Dictionary<string, Prototype.NetworkLobby.LobbyServerEntry> mAddressMatchDicitonary = new Dictionary<string, Prototype.NetworkLobby.LobbyServerEntry>();
	public Prototype.NetworkLobby.LobbyServerEntry serverEntryPrefab;


	public override void OnReceivedBroadcast(string fromAddress, string data)
	{
		base.OnReceivedBroadcast (fromAddress, data);

		if (mAddressMatchDicitonary.ContainsKey (fromAddress))
		{
			return;
		}

		Prototype.NetworkLobby.LobbyServerEntry obj = Instantiate (serverEntryPrefab);
		obj.setupLocalMatch("Local: "+fromAddress, () => {startCallback(fromAddress);});
		localMatches.Add (obj);
		mAddressMatchDicitonary.Add (fromAddress, obj);
		StartCoroutine (intermediate (fromAddress,obj));
	}

	public IEnumerator intermediate(string fromAddress, Prototype.NetworkLobby.LobbyServerEntry obj)
	{
		yield return new WaitForSeconds(2); 
		localMatches.Remove(obj); 
		mAddressMatchDicitonary.Remove(fromAddress);
	}
	
	public void startCallback(string data)
	{
		//var items = data.Split (':');
		lobbyManager.networkAddress = data;
		//lobbyManager.networkPort = Convert.ToInt32(items[2]);
		StopBroadcast ();
		lobbyManager.StartClient ();
	}
}

