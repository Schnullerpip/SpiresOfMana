using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LightningStrike : NetworkBehaviour
{
    enum State
    {
        FOLLOW, WARNING, STRIKE
    }

    [Tooltip("Approximately the time it will take to reach the target. A smaller value will reach the target faster.")]
    public float smoothTime = .3f;
	public float lifetime = 0.5f;

    public Projector blobShadow;
    public GameObject line;

    public FloatRange speed = new FloatRange(1.7f,2.3f);
    private float mSpeed;

    public float randomOffset = 0.5f;

    public float anticipationTime = 1;

    public FloatRange predictionTime = new FloatRange(0.2f, 1);

    public GameObject contactPrefab;

    [Tooltip("How much time of the anticipation should be dedicated to following the target")]
    [Range(0, 1)]
    public float followPercentage = 0.9f;

    public float strikeRadius = 1;
    public float shakeRadius = 10;
    public int damage = 10;
    public LayerMask hittingMasks;

    public AnimationCurve curve;
	public PlayerScript target;

    [Header("SFX")]
    public PitchingAudioClip[] clips;
    public AudioSource strikeSource;

	[SyncVar]
	private float mCurrentLifetime;
	private State mState;
 
    private Vector3 mRandomWander;

    private int mLerpProperyID;

    private void Awake()
    {
        //create a new material instance for every strike, 
        //because projectors dont create them on their own, like e.g. meshrenderer
        blobShadow.material = new Material(blobShadow.material);

        hits = new RaycastHit[5];

        mLerpProperyID = Shader.PropertyToID("_Lerp");
    }

    void LifeTimeEvaluation()
    {
        float f = 1 - ( -mCurrentLifetime / anticipationTime);

        if(f > followPercentage)
        {
            blobShadow.material.SetFloat(mLerpProperyID, 1);
        }
        else
        {
            blobShadow.material.SetFloat(mLerpProperyID, 0);
        }

        f = curve.Evaluate(f);

        blobShadow.orthographicSize = 1-f;
        //blobShadow.material.SetFloat("_Lerp", f);
    }

    void OnEnable()
	{
        vel = Vector3.zero;
        line.SetActive(false);
        blobShadow.orthographicSize = 0;
        blobShadow.gameObject.SetActive(true);

        mSpeed = speed.Random();

        if(isServer)
        {
            if(target)
            {
                Vector3 pos =
                    //target.transform.position;
                    target.movement.GetAnticipationPosition(predictionTime.Random());
                pos += (Random.insideUnitCircle * randomOffset).ToVector3xz();

                pos.y = 0;

                transform.position = pos;
            }
        }
		mRandomWander = Random.insideUnitCircle.normalized.ToVector3xz() * mSpeed;

        mState = State.FOLLOW;

		mCurrentLifetime = -anticipationTime;
	}

    Vector3 vel;

    private void Update()
    {
        if(isServer)
        {
            if(mState == State.FOLLOW)
            {
                if(mCurrentLifetime < - anticipationTime * (1-followPercentage))
    			{
                    if (target)
                    {
                        Vector3 targetPos = target.movement.GetAnticipationPosition(-mCurrentLifetime);
                        targetPos.y = 0;
                        transform.position =
                                     //Vector3.MoveTowards(transform.position, targetPos, mSpeed * Time.deltaTime * smoothTime);
                                     //Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
                                     Vector3.SmoothDamp(transform.position, targetPos, ref vel, smoothTime, mSpeed);
                                     //targetPos;

                        //Debug.DrawLine(transform.position,targetPos,Color.green);
                    }
                    else
                    {
                        transform.position = Vector3.SmoothDamp(transform.position, transform.position + mRandomWander, ref vel, smoothTime, mSpeed);
                    }
    			}
                else
                {
                    mState = State.WARNING;
                }
            }

            if(mState == State.WARNING)
            {
                if(mCurrentLifetime > 0)
                {
                    Damage();
                    RpcFlash();
                    mState = State.STRIKE;

                }     
            }
        }

		mCurrentLifetime += Time.deltaTime;
        LifeTimeEvaluation();
    }

    private RaycastHit[] hits;
    int hitAmount;

    void Damage()
    {
        target = null;

        Collider[] overlaps = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 40, shakeRadius, hittingMasks);

        //caching to avoid hitting the player multiple times. this is due to the fact the player has more than one collider
        List<HealthScript> cachedHealth = new List<HealthScript>();

        Vector3 temp;

        foreach (var coll in overlaps)
        {
            HealthScript health = coll.GetComponentInParent<HealthScript>();
            if(health && !cachedHealth.Contains(health))
            {
                temp = coll.transform.position;
                temp.y = 0;

                temp -= transform.position;

                //within the damage zone
                if (temp.sqrMagnitude < strikeRadius * strikeRadius)
                {
                    cachedHealth.Add(health);
                    health.TakeDamage(damage, this.GetType());
                }
                //the outer zone gets a camera shake
                else
                {
                    if (coll.attachedRigidbody.CompareTag("Player"))
                    {
                        //normalize the distance
                        temp /= shakeRadius;
                        //shake amount is defined as half the damage with a square falloff
                        float shake = (1 - temp.sqrMagnitude) * damage * 0.5f;
                        coll.attachedRigidbody.GetComponent<PlayerScript>().RpcShockPlayer(shake);
                    }
                }
            }
        }

        hitAmount = Physics.RaycastNonAlloc(transform.position + Vector3.up * 40, Vector3.down, hits, 40);

        for (int i = 0; i < hitAmount; ++i)
        {
            GameObject decal = PoolRegistry.GetInstance(contactPrefab, 20, 10);
            decal.transform.SetPositionAndRotation(hits[i].point, Quaternion.Euler(hits[i].normal));
            decal.SetActive(true);
        }
    }

    [ClientRpc]
    void RpcFlash()
    {
        StartCoroutine(Flash());

        strikeSource.transform.SetPosition(y: GameManager.instance.localPlayer.transform.position.y);

        PitchingAudioClip randomClip = clips.RandomElement();

        strikeSource.pitch = randomClip.GetRandomPitch();
        strikeSource.clip = randomClip.audioClip;
        strikeSource.Play();
    }

    IEnumerator Flash()
    {
        blobShadow.gameObject.SetActive(false);
        line.SetActive(true);

        yield return new WaitForSeconds(lifetime);

        line.SetActive(false);

        float soundDuration = strikeSource.clip.length;

        if (soundDuration > lifetime)
        {
			yield return new WaitForSeconds(lifetime - soundDuration);
        }
		
		NetworkServer.UnSpawn(this.gameObject);
		this.gameObject.SetActive(false);
	}
}
