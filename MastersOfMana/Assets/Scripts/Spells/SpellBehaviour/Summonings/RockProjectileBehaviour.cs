using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class RockProjectileBehaviour : A_ServerMoveableSummoning
{
    [SerializeField] private BoxCollider mHitCollider;
    [SerializeField] private SphereCollider mDetectCollider;
    [SerializeField] private float mRockVelocity;
    [SerializeField] private int mDamageAmount;
    [SerializeField] private float mRotationVelocity;
    [SerializeField] private float mPushForce;
    [SerializeField] private Vector3[] mRandomOffsets;
    private float mTimeCount = 0;

    [SyncVar] private GameObject casterObject;
    private PlayerScript caster;

    private delegate void OnTriggerBehaviour(Collider collider);
    private OnTriggerBehaviour behaviour;
    [SyncVar]
    private bool mRotateAroundCaster = true;
    [SyncVar]
    private Vector3 mOffset;
    private float mFixOffsetY = 1.5f;

    private List<GameObject> mAlreadyFound;

    public override void Execute(PlayerScript caster)
    {
        //get a rock instance
        RockProjectileBehaviour rp = PoolRegistry.Instantiate(this.gameObject).GetComponent<RockProjectileBehaviour>();

        //initialize it
        //rp.caster = caster;
        rp.mHitCollider.enabled = false;
        rp.mDetectCollider.enabled = true;
        rp.mRotateAroundCaster = true;
        rp.behaviour = rp.TriggerBehaviour_Detect;
        rp.casterObject = caster.gameObject;
        rp.caster = caster;
        //rp.transform.SetLayer(9);

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
        return new Vector3
        {
            x = Random.Range(0.0f, 1.0f) * (Random.value > 0.5f ? -1 : 1),
            y = Random.Range(0.0f, 1.0f) * (Random.value > 0.5f ? -1 : 1),
            z = Random.Range(0.0f, 1.0f) * (Random.value > 0.5f ? -1 : 1)
        }.normalized;
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
            mAlreadyFound = new List<GameObject>();
            mRigid = GetComponent<Rigidbody>();
        }
    }

    public override void OnStartClient()
    {
        if (!isServer)
        {
            caster = casterObject.GetComponent<PlayerScript>();
            mHitCollider.enabled = false;
            mDetectCollider.enabled = false;
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
        behaviour(collider);
    }

    private void TriggerBehaviour_Hit(Collider collider)
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

    private void TriggerBehaviour_Detect(Collider collider)
    {
        Rigidbody rigid = collider.attachedRigidbody;
        if (rigid && !mAlreadyFound.Contains(rigid.gameObject))
        {
            mAlreadyFound.Add(rigid.gameObject);
            PlayerScript opponent = rigid.GetComponent<PlayerScript>();
            if (opponent && opponent != caster)
            {
                //reset the already found list for further use
                mAlreadyFound = new List<GameObject>();


                //change the trigger enter behaviour
                transform.SetLayer(2);
                behaviour = TriggerBehaviour_Hit;
                mHitCollider.enabled = true;
                mDetectCollider.enabled = false;

                //set velocity to shoot towards enemy
                Vector3 projectileVelocity = Vector3.Normalize(opponent.movement.mRigidbody.worldCenterOfMass - transform.position)*mRockVelocity;

                RpcShoot(transform.position, projectileVelocity);
            }
        }
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