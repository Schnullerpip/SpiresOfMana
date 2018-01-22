using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Finds the nearest object beneath the caster and instantiates a wall, coming out of the floor
/// </summary>
public class EarthwallBehaviour : A_SummoningBehaviour {

    public float initialDistanceToCaster = 2;
    [SerializeField] private float mOffsetToHitGeometry;

    public AudioSource loopSource;
    public FloatRange volumeOverLife;
    public GameObject mCollisionReactionEffect;
    public AnimationCurve spawnScale;
    public Vector3 mOriginalScale;
    private HealthScript mHealthScript;
    public int damagePerSecond;
    protected Dictionary<ServerMoveable, int> mAlreadyCaught;
    public float instantEffectTriggerThreshold;
    public float dotProductThreshold;
    [SyncVar]
    private bool mPendingContactEffect = false;
    public float mOriginalVolume;

    //defines the loss factor with wich players bounce off of the shield
    public float VelocityLossFactor;

    void OnValidate()
    {
        mOriginalScale = transform.localScale;
    }

	public override void Preview (PlayerScript caster)
	{
		base.Preview(caster);

        preview.instance.SetAvailability(caster.CurrentSpellReady());
        Vector3 position;
        Quaternion rotation;
        GetSpawnPositionAndRotation(caster, out position, out rotation, true);
        preview.instance.MoveAndRotate(position, rotation);
	}

    private void GetSpawnPositionAndRotation(PlayerScript caster, out Vector3 position, out Quaternion rotation, bool isClientCall)
    {
		RaycastHit hit;
        Vector3 aimDirection = isClientCall ? GetAimClient(caster, out hit, false) : GetAimServer(caster, out hit, false);
        Vector3 expectedPos = caster.movement.mRigidbody.worldCenterOfMass + aimDirection * initialDistanceToCaster;

        caster.SetColliderIgnoreRaycast(true);
        if (Physics.Raycast(new Ray(caster.movement.mRigidbody.worldCenterOfMass, aimDirection), out hit))
        {
            if (hit.distance <= initialDistanceToCaster)
            {
                expectedPos = hit.point + (hit.normal * mOffsetToHitGeometry);
            }
            //nothing in the way of the shield origin
        }

        //now check wether ot nor its bounds would collide with something and emend the position accordingly
        var pos = expectedPos;
        pos += CorrigatePosition(pos, aimDirection);

        caster.SetColliderIgnoreRaycast(false);

        expectedPos = pos;

        position = expectedPos;
        rotation = Quaternion.LookRotation(aimDirection);
    }

    private Vector3 CorrigatePosition(Vector3 pos, Vector3 aimDirection)
    {
        return 
        EmendPositionTowards(pos, aimDirection, Vector3.down, () => transform.localScale.y/2.0f+0.2f) +
        EmendPositionTowards(pos, aimDirection, Vector3.up, () => transform.localScale.y/2.0f+0.2f) +
        EmendPositionTowards(pos, aimDirection, Vector3.left, () => transform.localScale.x/2.0f+0.2f) +
        EmendPositionTowards(pos, aimDirection, Vector3.right, () => transform.localScale.x/2.0f+0.2f);
    }

    private Vector3 EmendPositionTowards(Vector3 position, Vector3 direction, Vector3 towards, Func<float> supposedDistanceGetter)
    {
        RaycastHit hit;
        var dir = Quaternion.LookRotation(direction)*towards;
        var supposedDistance = supposedDistanceGetter();
        Debug.DrawRay(position, dir, Color.green);
        if (Physics.Raycast(new Ray(position, dir), out hit) && hit.distance < supposedDistance)
        {
            //towards (shields perspective) we have a collision -> emend the position accordingly
            return -1*dir*(supposedDistance - hit.distance);
        }
        return Vector3.zero;
    }

    public override void StopPreview(PlayerScript caster)
    {
        base.StopPreview(caster);
        preview.instance.Deactivate();
    }

    public override void Execute(PlayerScript caster)
    {
        EarthwallBehaviour wall = PoolRegistry.GetInstance(gameObject, 1, 1).GetComponent<EarthwallBehaviour>();
        wall.gameObject.SetActive(true);
        wall.casterObject = caster.gameObject;
        wall.caster = caster;

        Vector3 position;
        Quaternion rotation;
        GetSpawnPositionAndRotation(caster, out position, out rotation, false);
        wall.transform.position = position;
        wall.transform.rotation = rotation;
        NetworkServer.Spawn(wall.gameObject);


        //check whether an instant velocity change and contactEffect should be applied
        //if the player falls down rapidly he/she wont be able to catch him/herself with the shield, because the triggers will fall right through the colliders
        var y = caster.movement.GetVelocity().y;
        var dot = Vector3.Dot(Vector3.down, caster.GetCameraLookDirection());
        if (y < 0 && Mathf.Abs(y) >= instantEffectTriggerThreshold && dot >= dotProductThreshold)
        {
            wall.mPendingContactEffect = true;
            caster.movement.RpcInvertVelocity(VelocityLossFactor);
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
                //remove one if no more left - the object must be outside
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
            //else check whether it is allowed to move the object by server authority
            if (isServer)
            {
                sm = rigid.GetComponentInParent<ServerMoveable>();
                if (sm)
                {
                    if (!mAlreadyCaught.ContainsKey(sm))
                    {
                        //remember the servermoveable
                        mAlreadyCaught.Add(sm, 1);
                    }
                    else
                    {
                        mAlreadyCaught[sm] += 1;
                    }
                    //reflect the server moveable
                    sm.RpcInvertVelocity(1.0f);

                    //make sure reflected spells can hit/affect their casters
                    A_SpellBehaviour sb = sm.GetComponent<A_SpellBehaviour>();
                    if (sb && sb.GetCaster() != caster)
                    {
                        sb.SetCaster(caster);
                    }
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

    public void CollisionRoutine(Collision collision)
    {
        //if its a server moveable - invert its velocity (deflect/trampoline effect)
        Rigidbody rigid = collision.collider.attachedRigidbody;
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
                        player.movement.mRigidbody.velocity = -1 * collision.relativeVelocity * VelocityLossFactor;
                    }
                    else
                    {
                        mAlreadyCaught[player.movement] += 1;
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
    }

    public void CollisionExitRoutine(Collision collision)
    {
        Rigidbody rigid = collision.collider.attachedRigidbody;
        if (rigid)
        {
            ServerMoveable sm = rigid.GetComponent<ServerMoveable>();
            if (sm && mAlreadyCaught.ContainsKey(sm))
            {
                //remove one if no more left - the object must be outside
                if (--mAlreadyCaught[sm] <= 0)
                {
                    mAlreadyCaught.Remove(sm);
                }
            }
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

    //caches the timestamp for the last effect, so the next one can be invoked in away, that doesnt trash up the screen
    private float mTimeAtLastEffect;
    public float EffectIntervalInSeconds;
    private bool mFirstEffect;

    private void SpawnContactEffect()
    {
        var currentTimeStamp = Time.time;
        if (mFirstEffect || (currentTimeStamp - mTimeAtLastEffect >= EffectIntervalInSeconds))
        {
            mFirstEffect = false;
            GameObject go = PoolRegistry.GetInstance(mCollisionReactionEffect, 4, 4);
            go.transform.rotation = transform.rotation;
            go.transform.position = transform.position;
            go.SetActive(true);
            caster.StartCoroutine(DeactivateContactEffect(go));

            mTimeAtLastEffect = currentTimeStamp;
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
        mAlreadyCaught = new Dictionary<ServerMoveable, int>();
        mScaleCount = 0;
        mFirstEffect = true;
        mTimeAtLastEffect = 0;
        mDamageCount = 0;
        ScaleCountAddRoutine = Add;
        transform.localScale = mOriginalScale*spawnScale.Evaluate(mScaleCount);
    }

    private float mScaleCount, mDamageCount;
    private void Update()
    {
        //only relevant for visuals
        if (mPendingContactEffect)
        {
            mPendingContactEffect = false;
            SpawnContactEffect();
        }
        float factor = volumeOverLife.Lerp(mHealthScript.GetCurrentHealthPercentage());
        loopSource.volume = mOriginalVolume * factor;

        transform.localScale = mOriginalScale*spawnScale.Evaluate(mScaleCount);
        ScaleCountAddRoutine();
        mDamageCount += Time.deltaTime;

        if (isServer)
        {
            if (mDamageCount > 1.0f)
            {
               mDamageCount = 0;
               mHealthScript.TakeDamage(damagePerSecond, null, GetType());
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