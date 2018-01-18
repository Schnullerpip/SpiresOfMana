using UnityEngine;
using UnityEngine.UI;
using Rewired;
using System.Collections;

public class PostGameMenu : MonoBehaviour
{
    public Text text;
    public Button resumeButton;
    public float enableAfter = 1;
    private HUD mHud;

    public void Start()
    {
        mHud = GetComponentInParent<HUD>();
        
    }

    private IEnumerator EnableMenu()
    {
        resumeButton.onClick.RemoveAllListeners();
        yield return new WaitForSeconds(enableAfter);
        resumeButton.onClick.AddListener(mHud.ExitPostGameScreen);
        resumeButton.interactable = true;
    }

    public void OnEnable()
    {
        if (GameManager.instance.winnerID == GameManager.instance.localPlayer.netId.Value)
        {
            text.text = "You won!!!";
        }
        else
        {
            text.text = "You Lost!";
        }
        StartCoroutine(EnableMenu());
    }

    private void OnDisable()
    {
        resumeButton.interactable = false;
    }
}
