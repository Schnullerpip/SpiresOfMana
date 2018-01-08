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

        Debug.LogWarning("Remove code for stopping lava for release build!");
    }

    public void OnEnable()
    {
        GameManager.OnRoundStarted += RoundStarted;
        GameManager.OnHostEndedRound += HostRoundEnded;
        GameManager.OnRoundEnded += RoundEnded;
        mInstanceCoroutineDictionary = new Dictionary<HealthScript, Coroutine>();
    }

    public void OnDisable()
    {
        GameManager.OnRoundStarted -= RoundStarted;
        GameManager.OnHostEndedRound -= HostRoundEnded;
        GameManager.OnRoundEnded -= RoundEnded;
    }

    private static Dictionary<HealthScript, Coroutine> mInstanceCoroutineDictionary = new Dictionary<HealthScript, Coroutine>();

    public void OnTriggerEnter(Collider other)
    {
        //due to performance reasons (specifically lag) - we should treat the local player locally (players have local authority of their movement)

        //is it the local player?
        PlayerScript ps = other.GetComponentInParent<PlayerScript>();
        if (ps && GameManager.instance.localPlayer == ps && !mInstanceCoroutineDictionary.ContainsKey(ps.healthScript))
        {
            //it is the local player! move it! damage will be applied by the server!
            mInstanceCoroutineDictionary.Add(ps.healthScript, StartCoroutine(LavaEffectLocal(ps)));	
        }
        //if it is not a player handle it on the server only!
        else 
        {
            if (isServer)
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
    }

    void RoundEnded()
    {
        mLavaActive = false;
        //Vector3 newTrans = transform.position;
        //newTrans.y = mStartHeight;
        //transform.position = newTrans;
        mRenderer.material.SetFloat(mEmissionID, mEmissionStart);
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
                player.healthScript.TakeDamage(damagePerSecond, GetType());
            }
            else
            {
                player.healthScript.CmdTakeLocalLavaDamage(damagePerSecond);
            }

            PlayDamageSound(player.transform.position);

            if (player.healthScript.IsAlive())
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

            health.TakeDamage(damagePerSecond, this.GetType());
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

    private void FixedUpdate()
    {
        if (isServer && mLavaActive)
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                mLavaActive = false;
                HostRoundEnded();
            }

            Vector3 newTransformPosition = transform.position;
            float evaluation = lavaFlow.Evaluate(mRunTime);
            newTransformPosition.y = evaluation + mStartHeight;
            transform.position = newTransformPosition;

            emissionEval = emission.Evaluate(mRunTime);

            if(mNextOutbreakIndex < emission.length && mRunTime > emission.keys[mNextOutbreakIndex].time)
            {
                mNextOutbreakIndex += 4;
                RpcOutbreak();
            }

            mRunTime += Time.fixedDeltaTime;
        }
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
