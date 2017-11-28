using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playername : MonoBehaviour {

    public Text playerNameText;
    public PlayerScript player;

    public void Init()
    {
        playerNameText.text = player.playerName;
    }
}
