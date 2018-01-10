using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameMusicManager : MonoBehaviour {

    public LavaFloor lavafloor;

    public AnimationCurve curve;

    private AudioSource mSource;

    private void Awake()
    {
        mSource = GetComponent<AudioSource>();
    }

    void Update () 
    {
        mSource.volume = curve.Evaluate(lavafloor.GetHeightNormalized());
	}
}
