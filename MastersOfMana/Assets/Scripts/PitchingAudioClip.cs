using UnityEngine;

[System.Serializable]
public struct PitchingAudioClip
{
	public AudioClip audioClip;
	public FloatRange pitchtingRange;

	public float GetRandomPitch()
	{
		return pitchtingRange.Random();
	}
}