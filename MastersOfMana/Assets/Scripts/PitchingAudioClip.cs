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

    public void Play(AudioSource source)
    {
        source.pitch = GetRandomPitch();
        source.PlayOneShot(audioClip);
    }
}