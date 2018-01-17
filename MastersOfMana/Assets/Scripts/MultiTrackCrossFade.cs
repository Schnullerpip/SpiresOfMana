using System.Collections;
using UnityEngine;

/// <summary>
/// Multi track cross fade. Can crossfade between multiple AudioSources by using a normalized value.
/// </summary>
public class MultiTrackCrossFade : MonoBehaviour
{
    public AudioSource[] sources;

    [Range(0, 1)]
    public float interpolation;

    public int currentTrack = 0;
    private int mMasterSample;

    private float[] mInitialVols;

    private void Start()
    {
        mInitialVols = new float[sources.Length];

        for (int i = 0; i < sources.Length; ++i)
        {
            //reset current sample just in case
            sources[i].timeSamples = 0;
            //cache the initial volume settings
            mInitialVols[i] = sources[i].volume;
        }
    }

    private void Update()
    {
        //get the current interpolation value between 0 and the length of sources
        float overallLerp = (sources.Length - 1) * interpolation;

        //use this value as the one-true-value aka the master sample to sync all other sources (slaves)
        mMasterSample = sources[currentTrack].timeSamples;

        for (int i = 0; i < sources.Length; ++i)
        {
            //this has to be done, otherwise the audiosources will eventually become slighty out of sync, causing a phase shift
            sources[i].timeSamples = mMasterSample;

            //a linear crossfade, keeping the overall power at the same rate since audiosources volume is normalized (0.5 + 0.5 = 1) (this would not be the case in decible)
            // heres a picture of it :D
            // ‾‾‾\ /\ /‾‾‾
            //     x  x  
            // ___/_\/_\___
            float vol = Mathf.Clamp01(1 - Mathf.Abs(overallLerp - i)) * mInitialVols[i];
            sources[i].volume = vol;
            if (vol > 0.5f && currentTrack != i)
            {
                //the loudest source is considered the current track and will dictate the sample to sync to
                currentTrack = i;
            }
        }
    }

    /// <summary>
    /// Sets the interpolation.
    /// </summary>
    /// <param name="value">Value.</param>
    public void SetInterpolation(float value)
    {
        interpolation = Mathf.Clamp01(value);
    }

    /// <summary>
    /// Sets the track and changes the interpolation accordingly.
    /// </summary>
    /// <param name="trackIndex">Track index.</param>
    public void SetTrack(int trackIndex)
    {
        trackIndex = ValidateTrackIndex(trackIndex);
        interpolation = Lerp(trackIndex);
    }

    /// <summary>
    /// Sets the track and changes the interpolation accordingly over the specified time.
    /// </summary>
    /// <param name="trackIndex">Track index.</param>
    /// <param name="time">Time.</param>
    public void SetTrack(int trackIndex, float time)
    {
        trackIndex = ValidateTrackIndex(trackIndex);

        float desiredInterpolation = Lerp(trackIndex);
        float maxDelta = Mathf.Abs(interpolation - desiredInterpolation) / time;

        StopAllCoroutines();
        StartCoroutine(Transition(Lerp(trackIndex), maxDelta));
    }

    /// <summary>
    /// Sets the track and changes the interpolation with the specified speed in seconds.
    /// </summary>
    /// <param name="speed">Speed.</param>
    /// <param name="trackIndex">Track index.</param>
    public void SetTrack(float speed, int trackIndex)
    {
        trackIndex = ValidateTrackIndex(trackIndex);

        StopAllCoroutines();
        StartCoroutine(Transition(Lerp(trackIndex), speed));
    }

    private IEnumerator Transition(float desiredInterpolation, float maxDelta)
    {
        //since we're comparing floats its wise to check for the difference with epsilon (super small number)
        while (Mathf.Abs(interpolation - desiredInterpolation) > float.Epsilon)
        {
            interpolation = Mathf.MoveTowards(interpolation, desiredInterpolation, maxDelta * Time.deltaTime);
            yield return null;
        }

        //just in case set the value once more since the loop could maybe stop one iteration short or a floating point error got here somehow from somewhere
        interpolation = desiredInterpolation;
    }

    #region Helper
    /// <summary>
    /// Validates the index of the track.
    /// </summary>
    /// <returns>The track index.</returns>
    /// <param name="trackIndex">Track index.</param>
    private int ValidateTrackIndex(int trackIndex)
    {
        return Mathf.Clamp(trackIndex, 0, sources.Length - 1);
    }

    /// <summary>
    /// Return the interpolation value of the specified track index.
    /// </summary>
    /// <returns>The lerp.</returns>
    /// <param name="trackIndex">Track index.</param>
    private float Lerp(int trackIndex)
    {
        return trackIndex * 1.0f / (sources.Length - 1);
    }
    #endregion

    #region Editor
    private void OnValidate()
    {
        if (sources.Length == 0 || sources[0] == null || sources[0].clip == null)
        {
            return;
        }

        int samples = sources[0].clip.samples;

        for (int i = 1; i < sources.Length; ++i)
        {
            if (sources[i] == null || sources[i].clip == null)
            {
                continue;
            }

            if (sources[i].clip.samples != samples)
            {
                Debug.LogWarningFormat(sources[i].gameObject,
                                       "AudioSource \"{0}\" contains clip \"{1}\" that doesn't have the same amount of samples. Be aware that this script will not work properly with looping sounds with differing amount of samples.",
                                       sources[i].name, sources[i].clip.name);
            }
        }
    }
    #endregion
}
