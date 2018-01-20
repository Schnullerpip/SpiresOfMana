using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed : MonoBehaviour {

    public KillFeedItem killFeedItemPrefab;
    public int numOfPooledItems = 10;
    public SpellRegistry spellregistry;
    public Sprite lavaIcon;
    public Sprite fallIcon;
    public Sprite disconnectIcon;

    private List<KillFeedItem> feedItemPool = new List<KillFeedItem>();
    private int mCurrentIndex = 0;

    void Start()
    {
        GameManager.instance.mKillFeed = this;
        for(int i = 0; i < numOfPooledItems; i++)
        {
            feedItemPool.Add(Instantiate(killFeedItemPrefab, transform));
            feedItemPool[i].gameObject.SetActive(false);
        }
        GameManager.OnPlayerDied += CreateKillFeed;
        GameManager.OnHostEndedRound += ClearKilLFeed;
    }

    void OnDestroy()
    {
        GameManager.OnPlayerDied -= CreateKillFeed;
        GameManager.OnHostEndedRound -= ClearKilLFeed;
    }

    public void ClearKilLFeed()
    {
        foreach(KillFeedItem item in feedItemPool)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void CreateKillFeed(string killerName, System.Type damageSource, string deadPlayerName)
    {
        CreateKillFeed(killerName, damageSource.AssemblyQualifiedName, deadPlayerName);
    }

    public void CreateKillFeed(string killerName, string damageSource, string deadPlayerName)
    {
        Sprite icon;
        A_Spell spell = spellregistry.GetSpellByType(damageSource);
        if(spell)
        {
            icon = spell.icon;
        }
        else if(damageSource == GameManager.instance.GetLavaFloor().GetType().AssemblyQualifiedName)
        {
            icon = lavaIcon;
        }
        else
        {
            icon = fallIcon;
        }
        CreateItem(killerName, icon, deadPlayerName);
    }

    public void CreateDisconnectNotification(string disconnectedPlayer)
    {
        CreateItem("", disconnectIcon, disconnectedPlayer);
    }

    private void CreateItem(string playerName1, Sprite icon, string playerName2)
    {
        KillFeedItem item = GetItem();
        item.Init(playerName1, icon, playerName2);
        item.transform.SetParent(transform);
        item.gameObject.SetActive(true);
    }

    private KillFeedItem GetItem()
    {
        mCurrentIndex++;
        if (mCurrentIndex == feedItemPool.Count)
        {
            mCurrentIndex = 0;
        }
        return feedItemPool[mCurrentIndex];
    }
}
