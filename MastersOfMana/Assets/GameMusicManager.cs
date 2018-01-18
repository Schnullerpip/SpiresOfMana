using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour {

    public LavaFloor lavafloor;

    public AnimationCurve curve;

    public MultiTrackCrossFade multiTrackCrossFader;

    void Update () 
    {
        multiTrackCrossFader.SetInterpolation(curve.Evaluate(lavafloor.GetHeightNormalized()));
	}
}
