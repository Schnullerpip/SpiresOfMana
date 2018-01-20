using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour {

    public AnimationCurve curve;

	public MultiTrackCrossFade multiTrackCrossFader;
    public float maxDelta = 0.1f;

    private void Start()
    {
        GameManager.OnRoundEnded += ResetMusic;
    }

    void ResetMusic()
    {
        multiTrackCrossFader.CrossfadeToTrack(0,.25f);
    }

    void Update () 
    {
        if(!multiTrackCrossFader.IsTransitioning())
        {
			multiTrackCrossFader.MoveInterpolation(curve.Evaluate(GameManager.instance.GetTension()), maxDelta);
        }
	}
}
