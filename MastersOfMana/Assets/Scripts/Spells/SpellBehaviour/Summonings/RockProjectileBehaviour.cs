using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class RockProjectileBehaviour : A_ServerMoveableSummoning
{
    [SerializeField] private SphereCollider mHitCollider;
    [SerializeField] private float mRockVelocity;
    [SerializeField] private int mDamageAmount;

    [SerializeField] private float mRotationVelocity;//initial value
    [SyncVar]
    private float mIndividualRotationVelocity;//will diverge a little to give the rocks an individual touch

    [SerializeField] private float mPushForce;
    [SerializeField] private float mShootingReach;
    [SerializeField] private Vector3[] mRandomOffsets;
    [SerializeField] private Mesh[] mRandomRockMeshes;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private TrailRenderer spawnTrail;
    [SerializeField] private ParticleSystem mRockDustParticles;
    [SerializeField] private GameObject mCollisioinEffect;

    public AudioSource whooshSource;

    //for realizing a shooting order
    private RockProjectileBehaviour successor, previous;
    private bool token;
    public float shootFreqencyInSeconds;
    private float mShootCount;

    private float mTimeCount = 0;
    private List<PlayerScript> enemys;

    [SyncVar]
    private bool mRotateAroundCaster = true;
    [SyncVar]
    private Vector3 mOffset;

    //nice visuals
    [SyncVar]
    private Vector3 mRotationAxis;
    [SerializeField]
    private float mRotationSpeed;
    //will cache the spawnTrails Time property to be able to fade it out over time instead of just making it disappear
    public float spawnTrailTime;

    private static int mOffsetCount = 0;
    private static int mMeshCount = 0;

    //to be able to remember which stone was casted y which player
    private static Dictionary<PlayerScript, RockProjectileBehaviour> mShootingOrder = new Dictionary<PlayerScript, RockProjectileBehaviour>();

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

	    preview.instance.transform.localScale = mShootingReach * 2 * Vector3.one; // * 2 because we want to describe the radius rather than the diameter
        preview.instance.Move(caster.movement.mRigidbody.worldCenterOfMass);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();
	}


    public override void Execute(PlayerScript caster)
    {
        //get a rock instance
        RockProjectileBehaviour rp = PoolRegistry.GetInstance(gameObject, 1, 1).GetComponent<RockProjectileBehaviour>();

        //initialize it
        //rp.caster = caster;
        rp.mHitCollider.enabled = false;
        rp.mRotateAroundCaster = true;
        rp.casterObject = caster.gameObject;
        rp.caster = caster;
        rp.previous = null;
        rp.successor = null;
        rp.mIndividualRotationVelocity = mRotationVelocity + Random.Range(-2, 5);

        rp.mOffset = mRandomOffsets[mOffsetCount++];
        if (mOffsetCount >= mRandomOffsets.Length)
        {
            mOffsetCount = 0;
        }
        rp.RepositionRock();

        //gather all enemys for the new rockprojectile
        rp.enemys = new List<PlayerScript>();
        foreach (var p in GameManager.instance.players)
        {
            if (p != caster)
            {
                rp.enemys.Add(p);
            }
        }

        //spawn an explosion to make an entrance!
        GameObject spawnExplosion = PoolRegistry.GetInstance(mCollisioinEffect, rp.transform.position, rp.transform.rotation, 2, 4, Pool.PoolingStrategy.OnMissSubjoinElements, Pool.Activation.ReturnActivated);
        NetworkServer.Spawn(spawnExplosion);

        //if that caster casted his/her first rockprojectile create a new list to remember his projectiles
        if (mShootingOrder.ContainsKey(caster))
        {
            mShootingOrder[caster].successor = rp;
            rp.previous = mShootingOrder[caster];
            mShootingOrder[caster] = rp;
            rp.mShootCount = 0;
        }
        else
        {
            rp.mShootCount = shootFreqencyInSeconds;
            mShootingOrder.Add(caster, rp);
            rp.previous = null;
        }

        //spawn it
        rp.gameObject.SetActive(true);
        NetworkServer.Spawn(rp.gameObject);
    }

    public override void Awake()
    {
        base.Awake();
        whooshSource = GetComponent<AudioSource>();
    }


    [ContextMenu("InitializeRandomOffsets")]
    public void InitializeRandomOffsets()
    {
        for (var i = 0; i < mRandomOffsets.Length; ++i)
        {
            mRandomOffsets[i] = GetRandomOffset();
        }
    }

    private Vector3 GetRandomOffset()
    {
        Vector3 r = new Vector3
        {
            x = Random.Range(0.0f, 1.0f) * (Random.value > 0.5f ? -1 : 1),
            y = Random.Range(0.0f, 0.5f) * (Random.value > 0.5f ? -1 : 1),
            z = Random.Range(0.0f, 1.0f) * (Random.value > 0.5f ? -1 : 1)
        }.normalized;
        r.y += 0.45f;
        Vector2 xz = r.xz().normalized*0.8f;
        r.x = xz.x;
        r.z = xz.y;
        return r;
    }

    private void RepositionRock()
    {
        if (caster != null)
        {
            transform.SetPositionAndRotation(caster.movement.mRigidbody.worldCenterOfMass,
                Quaternion.AngleAxis(mTimeCount *mRotationSpeed, mRotationAxis) * 
                Quaternion.AngleAxis(mTimeCount * mIndividualRotationVelocity, Vector3.up));

            transform.Translate(mOffset);
        }
    }

    public void Start()
    {
        if (isServer)
        {
            mRigid = GetComponent<Rigidbody>();
            GameManager.OnRoundEnded += EndSpell;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();


        //assign one of the rock meshes
        GetComponentInChildren<MeshFilter>().mesh = mRandomRockMeshes[mMeshCount++];
        if (mMeshCount >= mRandomRockMeshes.Length)
        {
            mMeshCount = 0;
        }

    }

    public void OnEnable()
    {
        //for the visuals
        mRotationAxis = mOffset.normalized;
        spawnTrail.enabled = true;
        trail.enabled = false;
        mRockDustParticles.Play();
        spawnTrail.time = spawnTrailTime;//reset the spawnTrail.time to its initial value

    }


    public void Update()
    {
        //if we dont have a caster anymore - disappear (disconnected or player is dead)
        if (!caster)
        {
            return;
        }
        if (!caster.healthScript.IsAlive() && isServer)
        {
            Explode();
            return;
        }

        mTimeCount += Time.deltaTime;

        //after three seconds get rid of the spawn trail
        if (spawnTrail.enabled)
        {
            spawnTrail.time -= Time.deltaTime;
            if (mTimeCount >= spawnTrailTime)
            {
                spawnTrail.enabled = false;
            }
        }

        if (!isServer)
        {
            return;
        }

        if (!mRotateAroundCaster) return;

        //if it is this instances' turn to shoot towards enemies
        if (previous != null)
        {
            return; //this means an instance should be thrown before this instance
        }
        if((mShootCount+=Time.deltaTime) < shootFreqencyInSeconds)
        {
            return; //this rock instance should not yet be fired - delay is neccessary or else all rocks would be fired at once
        }

        //check for each enemy, wheather or not we should shoot towards them
        //get nearest enemy
        PlayerScript nearest = null;
        Vector3 casterPos = caster.movement.mRigidbody.worldCenterOfMass;
        float distance = 10000;
        for (var i = 0; i < enemys.Count; ++i)
        {
            if (enemys[i] != null)
            {
                float dist = Vector3.Distance(casterPos, enemys[i].movement.mRigidbody.worldCenterOfMass);
                if (dist < distance)
                {
                    nearest = enemys[i];
                    distance = dist;
                }
            }
        }
        //if nearest is in reach shoot it!
        if (distance <= mShootingReach && nearest)
        {
            //now if the stones direct path to the enemy is free, shoot it
            RaycastHit hit;
            Vector3 direction = Vector3.Normalize(nearest.transform.position - transform.position);
            if (Physics.Raycast(new Ray(transform.position, direction), out hit))
            {
                Rigidbody rigid = hit.rigidbody;
                if (rigid)
                {
                    PlayerScript enemy = rigid.GetComponentInParent<PlayerScript>();
                    if (enemy && enemy == nearest)
                    {
                        mRockDustParticles.Stop();
                        mRotateAroundCaster = false;
                        mHitCollider.enabled = true;
                        //set velocity to shoot towards enemy
                        Vector3 projectileVelocity = Vector3.Normalize(nearest.movement.mRigidbody.worldCenterOfMass - transform.position)*mRockVelocity;

                        //shoot the rock! -> make sure the shoting order stays legit
                        if (successor)
                        {
                            //if there is another instance after this one, inform it, that it is next
                            successor.previous = null;
                        }
                        else
                        {
                            //there is no successor -> make sure the next cast will be handled correctly
                            mShootingOrder.Remove(caster);
                        }

                        trail.enabled = true;
                        RpcShoot(transform.position, projectileVelocity);
                    }
                }
            }
        }
    }

    public void LateUpdate()
    {
        if (mRotateAroundCaster)
        {
            RepositionRock();
        }
    }
    protected override void ExecuteTriggerEnter_Host(Collider collider)
    {
        if (collider.isTrigger) return;

        HealthScript hs = collider.GetComponentInParent<HealthScript>();
        if (hs && hs != caster.healthScript)
        {
            //damage the hit object
            hs.TakeDamage(mDamageAmount, this.GetType());
        }

        ServerMoveable sm = collider.GetComponentInParent<ServerMoveable>();
        if (sm)
        {
            if (sm != caster.movement)
            {
                //we hit something, so disable the trail, that lines out our trajectorie
                trail.enabled = false;

                Vector3 direction = sm.mRigidbody.worldCenterOfMass - transform.position;

                //push the hit object 
                sm.RpcAddForce(direction.normalized*mPushForce, ForceMode.VelocityChange);
            }
            else
            {
                //we hit ourself -> just keep the rockProjectile in the air - dont disappear!
                return;
            }
        }

        //in any case we hit something (except for our caster) so we need to go away and make 'boom'!
        Explode();
    }

    public override void EndSpell()
    {
        StopPreview(caster);
        Explode();
    }


    private void Explode()
    {
        spawnTrail.enabled = false;

        GameObject collisionExplosion = PoolRegistry.GetInstance(mCollisioinEffect, transform.position,
            transform.rotation, 1, 1, Pool.PoolingStrategy.OnMissSubjoinElements, Pool.Activation.ReturnActivated);
        NetworkServer.Spawn(collisionExplosion);

        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);
    }


    [ClientRpc]
    private void RpcShoot(Vector3 position, Vector3 velocity)
    {
        //stop rotating around caster
        mRotateAroundCaster = false;
        //reset isKinematic 
        mRigid.isKinematic = false;

        mRigid.position = position;
        mRigid.velocity = velocity;

        //activate the trail
        trail.enabled = true;

        whooshSource.Play();
    }


    public void OnValidate()
    {
        for (var i = 0; i < mRandomOffsets.Length; ++i)
        {
            mRandomOffsets[i].Normalize();
        }
    }
}