using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour {

	public Text playerName;
	public Text playerStatus;

	public void SetName(string name)
	{
		playerName.text = name;
	}

	public void SetStatus(bool ready)
	{
		playerStatus.enabled = ready;
	}
}
