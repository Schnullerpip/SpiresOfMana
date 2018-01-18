using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class SpellDescription : MonoBehaviour {

    public Text spellName;
    public Text flavorText;
	public StatsIndicator damageIndicator;
	public StatsIndicator forceIndicator;
	public StatsIndicator cooldownIndicator;
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

		damageIndicator.SetIntensityLevel (mCurrentDescription.damage);
		forceIndicator.SetIntensityLevel (mCurrentDescription.force);
		cooldownIndicator.SetIntensityLevel (mCurrentDescription.cooldown);

        SetSpellName(mCurrentDescription.spellName);

        flavorText.text = mCurrentDescription.flavorText;

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

    private void OnDisable()
    {
        //reset description so its updated again on a restart
        mCurrentDescription = new A_Spell.SpellDescription();
    }

    private void SetFirstFrame(VideoPlayer vPlayer)
    {
        previewPlayer.frame = mStartFrame;
        previewPlayer.prepareCompleted -= SetFirstFrame;
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
