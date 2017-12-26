using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicMixer : MonoBehaviour {

    public AudioMixer mixer;

    public AnimationCurve transitionCurve;
    public float speed = 0.2f;

    public int startTrack = 0;

    public string[] tracks;

    private string mCurrentTrack;

    private void Awake()
    {
        mCurrentTrack = tracks[startTrack];
    }

    void Update()
    {
        for (int i = 0; i < tracks.Length; ++i)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if(tracks[i] != mCurrentTrack)
                {
                    StartCoroutine(TransitionTo(tracks[i], speed));
                }
            }
        }
    }

    private bool mBusy;

    IEnumerator TransitionTo(string to, float mySpeed)
    {
        yield return new WaitUntil(() => !mBusy);

        mBusy = true;
        float f = 0;
        while(f <= 1)
        {
            f += Time.deltaTime * mySpeed;

            float curvedF = transitionCurve.Evaluate(f);

            mixer.SetFloat(mCurrentTrack, Extensions.LinearToDecibel(1-curvedF));
            mixer.SetFloat(to, Extensions.LinearToDecibel(curvedF));

            yield return null;
        }

        mixer.SetFloat(mCurrentTrack, -80.0f);
        mixer.SetFloat(to, 0);

        mCurrentTrack = to;
        mBusy = false;
    }
}
