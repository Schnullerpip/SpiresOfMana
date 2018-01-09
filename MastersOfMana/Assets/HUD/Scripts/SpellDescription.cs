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

    private A_Spell.SpellDescription mCurrentDescription;
    private int mStartFrame, mEndFrame;

    public void SetDescription(A_Spell.SpellDescription description)
    {
        if(description.Equals(mCurrentDescription))
        {
            return;
        }

        mCurrentDescription = description;

        SetIndicator(damageIndicator, mCurrentDescription.damage);
        SetIndicator(cooldownIndicator, mCurrentDescription.cooldown);
        SetIndicator(forceIndicator, mCurrentDescription.force);
        SetSpellName(mCurrentDescription.spellName);
        mStartFrame = mCurrentDescription.beginFrame;
        mEndFrame = mCurrentDescription.endFrame;
        if (!previewPlayer.isPlaying)
        {
            previewPlayer.Play();
            previewPlayer.prepareCompleted += SetFirstFrame;
        }
        else
        { 
            previewPlayer.frame = mStartFrame;
        }
    }

    private void SetFirstFrame(VideoPlayer vPlayer)
    {
        previewPlayer.frame = mStartFrame;
        previewPlayer.prepareCompleted -= SetFirstFrame;
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

    private void Update()
    {
        if(previewPlayer.frame >= mEndFrame)
        {
            previewPlayer.frame = mStartFrame;
        }
    }
}
