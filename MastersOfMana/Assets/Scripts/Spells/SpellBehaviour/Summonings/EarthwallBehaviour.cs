using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Finds the nearest object beneath the caster and instantiates a wall, coming out of the floor
/// </summary>
public class EarthwallBehaviour : A_SummoningBehaviour {

    public float initialDistanceToCaster = 2;

    public EarthWallHealthScript health;
    public AudioSource loopSource;
    public FloatRange volumeOverLife;
    [SerializeField] private GameObject mCollisionReactionEffect;
    public AnimationCurve spawnScale;
    public Vector3 mOriginalScale;
    private HealthScript mHealthScript;
    public int damagePerSecond;
    private List<GameObject> mAlreadyCaught;

    void OnValidate()
    {
        mOriginalScale = transform.lossyScale;
    }


    public override void Preview(PlayerScript caster)
    {
        base.Preview(caster);

        preview.instance.SetAvailability(caster.CurrentSpellReady());
        preview.instance.MoveAndRotate(caster.movement.mRigidbody.worldCenterOfMass + GetAimClient(caster) * initialDistanceToCaster, 
                                       Quaternion.LookRotation(GetAimClient(caster)));
    }

    public override void StopPreview(PlayerScript caster)
    {
        base.StopPreview(caster);
        preview.instance.Deactivate();
    }

    public override void Execute(PlayerScript caster)
    {
        EarthwallBehaviour wall =
            PoolRegistry.GetInstance(gameObject,
                caster.movement.mRigidbody.worldCenterOfMass + GetAimServer(caster)*initialDistanceToCaster,
                Quaternion.LookRotation(GetAimServer(caster)), 4, 4).GetComponent<EarthwallBehaviour>();
        wall.gameObject.SetActive(true);
        wall.casterObject = caster.gameObject;
        wall.caster = caster;
        NetworkServer.Spawn(wall.gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        //no effect with players
        bool hitCaster = false;
        if (!(hitCaster = (collider.attachedRigidbody && collider.attachedRigidbody.CompareTag("Player"))))
        {
            //contact reaction
            //create an contatreaction effect object
            GameObject go = PoolRegistry.GetInstance(mCollisionReactionEffect, 4, 4);
            go.transform.rotation = transform.rotation;
            go.transform.position = transform.position;
            go.SetActive(true);
            caster.StartCoroutine(DeactivateContactEffect(go));
        }

        //if its a server moveable - invert its velocity (deflect/trampoline effect)
        //TODO somehow doesnt really work with players (trampoline effect)
        if (isServer && !mAlreadyCaught.Contains(collider.gameObject))
        {
            if (!hitCaster)
            {
                mAlreadyCaught.Add(collider.gameObject);
            }

            Rigidbody rigid = collider.attachedRigidbody;
            if (rigid)
            {
                ServerMoveable sm = rigid.GetComponentInParent<ServerMoveable>();
                if (sm)
                {
                    sm.RpcInvertVelocity();
                }
            }
        }
    }


    IEnumerator DeactivateContactEffect(GameObject go)
    {
        yield return new WaitForSeconds(3.5f);
        go.SetActive(false);
    }

    void Start()
    {
        mHealthScript = GetComponent<HealthScript>();
    }

    void OnEnable()
    {
        mAlreadyCaught = new List<GameObject>();
        mScaleCount = 0;
        mDamageCount = 0;
        ScaleCountAddRoutine = Add;
        transform.localScale = mOriginalScale*spawnScale.Evaluate(mScaleCount);
    }

    private float mScaleCount, mDamageCount;
    private void Update()
    {
        loopSource.volume = volumeOverLife.Lerp(health.GetCurrentHealth() * 1.0f / health.GetMaxHealth() * 1.0f);

        transform.localScale = mOriginalScale*spawnScale.Evaluate(mScaleCount);
        ScaleCountAddRoutine();
        mDamageCount += Time.deltaTime;

        if (isServer)
        {
            if (mDamageCount > 1.0f)
            {
               mDamageCount = 0;
               health.TakeDamage(damagePerSecond, GetType());
            }

            if (mScaleCount <= 0)
            {
                gameObject.SetActive(false);
                NetworkServer.UnSpawn(gameObject);
            }
        }
    }

    private delegate void ScaleCountAdditionRoutine();
    private ScaleCountAdditionRoutine ScaleCountAddRoutine;

    private void Add()
    {
        mScaleCount += Time.deltaTime;
    }
    private void Substract()
    {
        mScaleCount -= Time.deltaTime;
    }

    public override void EndSpell()
    {
        base.EndSpell();

        StopPreview(caster);

        mScaleCount = 0.2f;
        ScaleCountAddRoutine = Substract;
    }
}