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
    private Dictionary<ServerMoveable, int> mAlreadyCaught;
    public float instantEffectTriggerThreshold;
    public float dotProductThreshold;
    [SyncVar]
    private bool mPendingContactEffect = false;

    void OnValidate()
    {
        mOriginalScale = transform.lossyScale;
    }


    public override void Preview(PlayerScript caster)
    {
        base.Preview(caster);

        preview.instance.transform.localScale = mOriginalScale;
        preview.instance.SetAvailability(caster.CurrentSpellReady());


        RaycastHit hit;
		if(caster.HandTransformIsObscured(out hit))
		{
            preview.instance.MoveAndRotate(hit.point, caster.aim.currentLookRotation);
			return;
		}

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


        //check whether an instant velocity change and contactEffect should be applied
        //if the player falls down rapidly he/she wont be able to catch him/herself with the shield, because the triggers will fall right through the colliders
        var y = caster.movement.GetVelocity().y;
        var dot = Vector3.Dot(Vector3.down, caster.GetCameraLookDirection());
        if (y < 0 && Mathf.Abs(y) >= instantEffectTriggerThreshold && dot >= dotProductThreshold)
        {
            wall.mPendingContactEffect = true;
            caster.movement.RpcInvertVelocity();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        Rigidbody rigid = collider.attachedRigidbody;
        if (rigid)
        {
            ServerMoveable sm = rigid.GetComponent<ServerMoveable>();
            if (sm && mAlreadyCaught.ContainsKey(sm))
            {
                if (--mAlreadyCaught[sm] <= 0)
                {
                    mAlreadyCaught.Remove(sm);
                }
            }
        }
    }

    new void OnTriggerEnter(Collider collider)
    {
        //if its a server moveable - invert its velocity (deflect/trampoline effect)
        Rigidbody rigid = collider.attachedRigidbody;
        ServerMoveable sm = null;
        if (rigid)
        {
            //handle local effect -> local player bouncing (cause of local authority on players)
            PlayerScript player = rigid.GetComponent<PlayerScript>();
            //if its the local player, then move it locally
            if (player)
            {
                if (player.isLocalPlayer)
                {
                    if (!mAlreadyCaught.ContainsKey(player.movement))
                    {
                        mAlreadyCaught.Add(player.movement, 1);
                        player.movement.mRigidbody.velocity *= -1;
                    }
                    else
                    {
                        mAlreadyCaught[player.movement] += 1;
                    }
                }
            }
            //else check whether it is allowed to move the object by server authority
            else if (isServer)
            {
                sm = rigid.GetComponentInParent<ServerMoveable>();
                if (sm && !mAlreadyCaught.ContainsKey(sm))
                {
                    //remember the servermoveable
                    mAlreadyCaught.Add(sm, 1);

                    //reflect the server moveable
                    sm.RpcInvertVelocity();
                }
            }
        }

        //contact reaction
        //create a contatreaction effect object
        SpawnContactEffect();
        //make sure the client reacts upon even, if the lag lets the clientside think no collision hapened
        if (isServer && sm)
        {
            RpcSpawnContactEffect(sm.gameObject);
        }
    }

    [ClientRpc]
    private void RpcSpawnContactEffect(GameObject collidingObject)
    {
        ServerMoveable sm = collidingObject.GetComponent<ServerMoveable>();
        if (collidingObject && !mAlreadyCaught.ContainsKey(sm))
        {
            SpawnContactEffect();
        }
    }

    private void SpawnContactEffect()
    {
            GameObject go = PoolRegistry.GetInstance(mCollisionReactionEffect, 4, 4);
            go.transform.rotation = transform.rotation;
            go.transform.position = transform.position;
            go.SetActive(true);
            caster.StartCoroutine(DeactivateContactEffect(go));
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
        mAlreadyCaught = new Dictionary<ServerMoveable, int>();
        mScaleCount = 0;
        mDamageCount = 0;
        ScaleCountAddRoutine = Add;
        transform.localScale = mOriginalScale*spawnScale.Evaluate(mScaleCount);
    }

    private float mScaleCount, mDamageCount;
    private void Update()
    {
        if (mPendingContactEffect)
        {
            mPendingContactEffect = false;
            SpawnContactEffect();
        }
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

        if (isServer)
        {
            RpcChangeScaleCount();
        }
    }

    [ClientRpc]
    private void RpcChangeScaleCount()
    {
        ScaleCountAddRoutine = Substract;
    }
}