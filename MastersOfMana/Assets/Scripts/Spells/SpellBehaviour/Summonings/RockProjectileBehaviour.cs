using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class RockProjectileBehaviour : A_ServerMoveableSummoning
{
    [SerializeField] private BoxCollider mHitCollider;
    [SerializeField] private float mRockVelocity;
    [SerializeField] private int mDamageAmount;
    [SerializeField] private float mRotationVelocity;
    [SerializeField] private float mPushForce;
    [SerializeField] private float mShootingReach;
    [SerializeField] private Vector3[] mRandomOffsets;
    private float mTimeCount = 0;
    private List<PlayerScript> enemys;

    [SyncVar] private GameObject casterObject;
    private PlayerScript caster;

    [SyncVar]
    private bool mRotateAroundCaster = true;
    [SyncVar]
    private Vector3 mOffset;

    public override void Execute(PlayerScript caster)
    {
        //get a rock instance
        RockProjectileBehaviour rp = PoolRegistry.Instantiate(this.gameObject).GetComponent<RockProjectileBehaviour>();

        //initialize it
        //rp.caster = caster;
        rp.mHitCollider.enabled = false;
        rp.mRotateAroundCaster = true;
        rp.casterObject = caster.gameObject;
        rp.caster = caster;

        rp.mOffset = GetRandomOffset();
        rp.RepositionRock();

        //spawn it
        rp.gameObject.SetActive(true);
        NetworkServer.Spawn(rp.gameObject);
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
        r.y += 0.3f;
        Vector2 xz = r.xz().normalized*0.8f;
        r.x = xz.x;
        r.z = xz.y;
        return r;
    }

    private void RepositionRock()
    {
        transform.SetPositionAndRotation(caster.movement.mRigidbody.worldCenterOfMass, Quaternion.AngleAxis(mTimeCount * mRotationVelocity, Vector3.up));
        transform.Translate(mOffset);
    }

    public void Start()
    {
        if (isServer)
        {
            mRigid = GetComponent<Rigidbody>();
        }
    }

    public override void OnStartClient()
    {

        //gather all the enemies
        enemys = new List<PlayerScript>();
        foreach (var p in GameManager.instance.mPlayers)
        {
            if (p != caster)
            {
                enemys.Add(p);
            }
        }

        if (!isServer)
        {
            caster = casterObject.GetComponent<PlayerScript>();
            mHitCollider.enabled = false;
        }
    }


    public void Update()
    {
        //as long as we do not have a caster yet - do nothing
        if (!caster)
        {
            return;
        }

        mTimeCount += Time.deltaTime;

        if (!isServer)
        {
            return;
        }

        if (!mRotateAroundCaster) return;

        //check for each enemy, wheather or not we should shoot towards them
        //get nearest enemy
        PlayerScript nearest = null;
        float distance = 10000;
        for (var i = 0; i < enemys.Count; ++i)
        {
            float dist = Vector3.Distance(transform.position, enemys[i].movement.mRigidbody.worldCenterOfMass);
            if (dist < distance)
            {
                nearest = enemys[i];
                distance = dist;
            }
        }
        //if nearest is in reach shoot it!
        if (distance < mShootingReach)
        {
            //now if the stones direct path to the enemy is free, shoot it
            RaycastHit hit;
            Vector3 direction = Vector3.Normalize(nearest.transform.position - transform.position);
            if (Physics.Raycast(new Ray(transform.position + direction*1.5f, direction), out hit))
            {
                Rigidbody rigid = hit.rigidbody;
                if (rigid)
                {
                    PlayerScript enemy = rigid.GetComponentInParent<PlayerScript>();
                    if (enemy && enemy == nearest)
                    {
                        mRotateAroundCaster = false;
                        mHitCollider.enabled = true;
                        //set velocity to shoot towards enemy
                        Vector3 projectileVelocity = Vector3.Normalize(nearest.movement.mRigidbody.worldCenterOfMass - transform.position)*mRockVelocity;

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
            hs.TakeDamage(mDamageAmount);
        }

        ServerMoveable sm = collider.GetComponentInParent<ServerMoveable>();
        if (sm )
        {
            if (sm != caster.movement)
            {
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

		gameObject.SetActive(false);
        caster = null;
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
    }

    public void OnValidate()
    {
        for (var i = 0; i < mRandomOffsets.Length; ++i)
        {
            mRandomOffsets[i].Normalize();
        }
    }
}