using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LavaFloor : NetworkBehaviour
{
    public int damagePerSecond;
    public AnimationCurve lavaFlow;

    public float repelForce = 10;

    private float mStartHeight;
    private float mRunTime = 0;
    private bool mLavaActive = false;

    [Header("Emission Animation")]
    [CurveDisplay]
    public AnimationCurve emission;
    public float emissionAmplifier = 4;
    public float rampTime = 0.5f;

    [SyncVar(hook = "EmissionHook")]
    private float emissionEval;

    private Renderer mRenderer;
    private float mEmissionStart;
    private int mEmissionID;
    private int mNextOutbreakIndex = 1;

    [Header("SFX")]
    public AudioSource bubblingSource;
    public AudioSource burnSource;
    public PitchingAudioClip[] burnClips;
    public AudioSource outbreakSource;

    private void Awake()
    {
        mRenderer = GetComponentInChildren<Renderer>();
        mEmissionID = Shader.PropertyToID("_EmissionStrength");
        mEmissionStart = mRenderer.material.GetFloat(mEmissionID);
    }

    public void Start()
    {
        mStartHeight = transform.position.y;

        Debug.LogWarning("Shift + L to stop lava. Remove this code for release build!");
    }

    public void OnEnable()
    {
        GameManager.OnPreGameAnimationFinished += RoundStarted;
        GameManager.OnHostEndedRound += HostRoundEnded;
        GameManager.OnRoundEnded += RoundEnded;
        mInstanceCoroutineDictionary = new Dictionary<HealthScript, Coroutine>();
        mCanSendSignal = true;
        mStopSignaling = false;
        mRiseCount = 4; //the first three keyframes need to be skipped
    }


    public void OnDisable()
    {
        GameManager.OnPreGameAnimationFinished -= RoundStarted;
        GameManager.OnHostEndedRound -= HostRoundEnded;
        GameManager.OnRoundEnded -= RoundEnded;
    }

    private static Dictionary<HealthScript, Coroutine> mInstanceCoroutineDictionary = new Dictionary<HealthScript, Coroutine>();

    public void OnTriggerEnter(Collider other)
    {
        //due to performance reasons (specifically lag) - we should treat the local player locally (players have local authority of their movement)

        //is it the local player?
        PlayerScript ps = other.GetComponentInParent<PlayerScript>();
        ParalysisBehaviour pb = other.GetComponent<ParalysisBehaviour>();
        if (!pb && ps && GameManager.instance.localPlayer == ps && !mInstanceCoroutineDictionary.ContainsKey(ps.healthScript))
        {
            //it is the local player! move it! damage will be applied by the server!
            mInstanceCoroutineDictionary.Add(ps.healthScript, StartCoroutine(LavaEffectLocal(ps)));	
        }
        //if it is not a player handle it on the server only!
        else if (isServer)
        {
            HealthScript health = other.GetComponentInParent<HealthScript>();
            if (health && health.IsAlive() && !mInstanceCoroutineDictionary.ContainsKey(health))
            {
                ServerMoveable sm = other.GetComponentInParent<ServerMoveable>();
                //Remember which server moveable this coroutine belongs to
                mInstanceCoroutineDictionary.Add(health, StartCoroutine(LavaEffectServer(health, sm)));
            }
        }
    }

    void RoundStarted()
    {
        mLavaActive = true;
        mNextOutbreakIndex = 1;
    }

    void HostRoundEnded()
    {
        mRunTime = 0;
        Vector3 newTransformPosition = transform.position;
        float evaluation = lavaFlow.Evaluate(mRunTime);
        newTransformPosition.y = evaluation + mStartHeight;
        transform.position = newTransformPosition;
		StopAllCoroutines ();
		mInstanceCoroutineDictionary.Clear ();
    }

    void RoundEnded()
    {
        mLavaActive = false;
        //Vector3 newTrans = transform.position;
        //newTrans.y = mStartHeight;
        //transform.position = newTrans;
        mRenderer.material.SetFloat(mEmissionID, mEmissionStart);

        //Lava RAS
        mCanSendSignal = true;
        mStopSignaling = false;
        mRiseCount = 4; //the first three keyframes need to be skipped
    }

    public void OnTriggerExit(Collider other)
    {
        //Check Healthscript to only trigger once per player
        HealthScript health = other.GetComponentInParent<HealthScript>();
        if (health && mInstanceCoroutineDictionary.ContainsKey(health))
        {
            //the coroutine might have stopped itself already - check that, else stop it
            Coroutine coR;
            mInstanceCoroutineDictionary.TryGetValue(health, out coR);
            if (coR != null)
            {
                StopCoroutine(coR);
            }
            mInstanceCoroutineDictionary.Remove(health);
        }
    }

    public IEnumerator LavaEffectLocal(PlayerScript player)
    {
        while (player.healthScript.IsAlive() && mLavaActive)
        {
            //if (!player.healthScript.IsAlive())
            //{
            //    StopCoroutine(mInstanceCoroutineDictionary[player.healthScript]);
            //    mInstanceCoroutineDictionary.Remove(player.healthScript);
            //    yield break;
            //}

            if (isServer)
            {
                player.healthScript.TakeDamage(damagePerSecond, player, GetType());
            }
            else
            {
                player.healthScript.CmdTakeLocalLavaDamage(damagePerSecond);
            }

            PlayDamageSound(player.transform.position);

            if (player.healthScript.IsAlive() && player.movement.GetMovementAllowed())
            {
                //shoot the moveable into the sky to make it jump mario-ayayayayay-style
                Vector3 vel = player.movement.mRigidbody.velocity;
                vel.y = repelForce;
                player.movement.mRigidbody.velocity = vel;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator LavaEffectServer(HealthScript health, ServerMoveable sm = null)
    {
        while (enabled && mLavaActive)
        {
            if (!health.IsAlive())
            {
                StopCoroutine(mInstanceCoroutineDictionary[health]);
                mInstanceCoroutineDictionary.Remove(health);
                yield break;
            }

            //null because this routine should normally never be called on a player
            health.TakeDamage(damagePerSecond, null, GetType());
            RpcPlayDamageSound(health.transform.position);

            if (sm && health.IsAlive())
            {
                //shoot the moveable into the sky to make it jump mario-ayayayayay-style
                sm.RpcSetVelocityY(repelForce);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    [ClientRpc]
    private void RpcPlayDamageSound(Vector3 position)
    {
        PlayDamageSound(position);
    }

    private void PlayDamageSound(Vector3 position)
    {
        burnSource.transform.position = position;
        PitchingAudioClip clip = burnClips.RandomElement();
        burnSource.pitch = clip.GetRandomPitch();
        burnSource.PlayOneShot(clip.audioClip);
    }

    //-------------------Lava RAS---------------------------
    [Header("Lava Rapid Alert System")]
    [SerializeField] private float RAS_Offset;

    private int mRiseCount = 4;
    private bool mCanSendSignal, mStopSignaling;


    public delegate void LavaWillRiseSoon();
    /// <summary>
    /// everyone, who wants to be informed when the lava will rise soon, should register on this event
    /// </summary>
    public event LavaWillRiseSoon OnLavaWillRise;

    public delegate void LavaStoppedRising();
    /// <summary>
    /// everyone, who wants to be informed when the lava stopped rising, should register on this event
    /// </summary>
    public event LavaStoppedRising OnLavaStoppedRising;
    //------------------------------------------------------


    private void FixedUpdate()
    {
        if (isServer && mLavaActive)
        {
            if(Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftShift))
            {
                mLavaActive = false;
                HostRoundEnded();
            }

            Vector3 newTransformPosition = transform.position;
            float evaluation = lavaFlow.Evaluate(mRunTime);
            newTransformPosition.y = evaluation + mStartHeight;
            transform.position = newTransformPosition;

            emissionEval = emission.Evaluate(mRunTime);

            //lava RAS (warning when lava is about to rise - look into the future according to RAS_Offset
            //enables the detection, so only oe event is send whenever the detection returns positive
            if (!mStopSignaling && (mRunTime >= emission.keys[mRiseCount].time))
            {
                mCanSendSignal = true;
                //inform listeners, that lava has stopped rising
                if (OnLavaStoppedRising != null)
                {
                    RpcTriggerOnLavaStoppedRising();
                }

                //reinitialize the detection
                if (mRiseCount + 4 >= emission.keys.Length)
                {
                    mStopSignaling = true;
                }
                else
                {
                    mRiseCount += 4;
                }
            }
            //detects wether the lava will rise soon and informs the listeners
            if (mCanSendSignal && ((int)emissionEval == 0) && (emission.Evaluate(mRunTime + RAS_Offset) > 0))
            {
                mCanSendSignal = false;
                //looking into the future by RAS_Offset we actually found, that the lava will rise! -> inform everyone, that needs to know
                if (OnLavaWillRise != null)
                {
                    RpcTriggerOnLavaWillRiseSoon();
                }
            }

            if(mNextOutbreakIndex < emission.length && mRunTime > emission.keys[mNextOutbreakIndex].time)
            {
                mNextOutbreakIndex += 4;
                RpcOutbreak();
            }


            mRunTime += Time.fixedDeltaTime;
        }
    }

    [ClientRpc]
    private void RpcTriggerOnLavaWillRiseSoon()
    {
        OnLavaWillRise();
    }

    [ClientRpc]
    private void RpcTriggerOnLavaStoppedRising()
    {
        OnLavaStoppedRising();
    }

    private void EmissionHook(float emissionValue)
    {
        this.emissionEval = emissionValue;
        mRenderer.material.SetFloat(mEmissionID, mEmissionStart + emissionEval * emissionAmplifier);
    }

    [ClientRpc]
    private void RpcOutbreak()
    {
        outbreakSource.Play();
    }

    public float GetHeightNormalized()
    {
        return (transform.position.y - mStartHeight) / lavaFlow.keys.LastElement().value;
    }

    private void OnValidate()
    {
        AnimationCurve temp = new AnimationCurve();

        for (int i = 0; i < lavaFlow.length; ++i)
        {
            Keyframe key = new Keyframe(lavaFlow.keys[i].time, 0, 0, 0);
            temp.AddKey(key);

            if (i > 0 && i < lavaFlow.length - 1)
            {
                if (i % 2 == 1)
                {
                    key = new Keyframe(lavaFlow.keys[i].time + rampTime, 1, 0, 0);
                }
                else
                {
                    key = new Keyframe(lavaFlow.keys[i].time - rampTime, 1, 0, 0);
                }

                temp.AddKey(key);
            }
        }

        emission.keys = temp.keys;
    }
}
