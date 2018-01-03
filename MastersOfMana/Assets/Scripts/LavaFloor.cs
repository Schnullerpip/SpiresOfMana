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
        GameManager.OnRoundEnded += RoundEnded;
    }

    public void OnDisable()
    {
        GameManager.OnRoundStarted -= RoundStarted;
        GameManager.OnRoundEnded -= RoundEnded;
    }

    private Dictionary<HealthScript, Coroutine> mInstanceCoroutineDictionary = new Dictionary<HealthScript, Coroutine>();

    public void OnTriggerEnter(Collider other)
    {
        if(!isServer)
        {
            return;
        }
        //Check if collision with player
        //Check FeetCollider to only trigger once per player
        HealthScript health = other.GetComponentInParent<HealthScript>();
        if (health && health.IsAlive() && !mInstanceCoroutineDictionary.ContainsKey(health))
        {
            ServerMoveable sm = other.GetComponentInParent<ServerMoveable>();
            //Remember which player this coroutine belongs to
            mInstanceCoroutineDictionary.Add(health, StartCoroutine(DealDamage(health, sm)));	
        }
    }

    void RoundStarted()
    {
        mLavaActive = true;
        mRunTime = 0;
        mNextOutbreakIndex = 1;
    }

    void RoundEnded()
    {
        mLavaActive = false;
        Vector3 newTrans = transform.position;
        newTrans.y = mStartHeight;
        transform.position = newTrans;
        mRenderer.material.SetFloat(mEmissionID, mEmissionStart);

    }

    public void OnTriggerExit(Collider other)
    {
        if (!isServer)
        {
            return;
        }
        //Check Healthscript to only trigger once per player
        HealthScript health = other.GetComponentInParent<HealthScript>();
        if (health && mInstanceCoroutineDictionary.ContainsKey(health))
        {
            StopCoroutine(mInstanceCoroutineDictionary[health]);
            mInstanceCoroutineDictionary.Remove(health);
        }
    }

    public IEnumerator DealDamage(HealthScript health, ServerMoveable sm = null)
    {
        while (enabled)
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
                RoundEnded();
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
