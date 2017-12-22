using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class SpellDescription : MonoBehaviour {

    public Text spellName;
    public Transform damageIndicator;
    public Transform forceIndicator;
    public Transform cooldownIndicator;
    public VideoPlayer previewPlayer;

    private string defaultStats;

    public void SetDescription(A_Spell.SpellDescription description)
    {
        SetIndicator(damageIndicator, description.damage);
        SetIndicator(cooldownIndicator, description.cooldown);
        SetIndicator(forceIndicator, description.force);
        SetSpellName(description.spellName);
        previewPlayer.clip = description.video;
    }

    public void SetIndicator(Transform indicator, int level)
    {
        for(int i = 0; i < indicator.childCount; i++)
        {
            indicator.GetChild(i).gameObject.SetActive(i<level);
        }
    }

    public void SetSpellName(string name)
    {
        spellName.text = name;
    }
}
