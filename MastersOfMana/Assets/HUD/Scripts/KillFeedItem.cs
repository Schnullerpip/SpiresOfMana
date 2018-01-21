using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour {

    public Text playerName1;
    public Text playerName2;
    public Image spellIcon;
    public float fadingSpeed;
    public CanvasGroup canvasGroup;

    void OnEnable()
    {
        canvasGroup.alpha = 1;
    }

    public void Init(string killerName, Sprite spellSprite, string deadPlayerName)
    {
        //if (enabled)//Otherwise leaving the game will cause the disconnect message to be displayed, but this killfeeditem was already destroyed
        //{
            playerName1.text = killerName;
            spellIcon.sprite = spellSprite;
            playerName2.text = deadPlayerName;
        //}
    }
	
	// Update is called once per frame
	void Update () {
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0, fadingSpeed);
        if (canvasGroup.alpha == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
