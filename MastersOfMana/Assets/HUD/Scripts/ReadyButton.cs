using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour {

    [Header("Ready Colors")]
    public ButtonColorSet readyColor;
    [Header("Unready Colors")]
    public ButtonColorSet unreadyColor;

    public string readyString = "READY";
    public string unreadyString = "UNREADY";

    public Button readyButton;
    private Text buttonLabel;

    void Start()
    {
        buttonLabel = readyButton.GetComponentInChildren<Text>();
    }

    void OnDisable()
    {
        SetReadyState(true);
    }

	public void SetReadyState(bool ready)
    {
        if (ready)
        {
            buttonLabel.text = readyString;
            readyButton.colors = readyColor.colors;
        }
        else
        {
            buttonLabel.text = unreadyString;
            readyButton.colors = unreadyColor.colors;
        }
    }
}
