using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class FistOfFuryBehaviour : A_SummoningBehaviour
{
    private PlayerScript caster;

    [SerializeField] private float explosionAmplitude;
    [SerializeField] private Explosion mExplosion;
    [SerializeField] private ParticleSystem mTrail;

    [SerializeField] private float mExplosionForce;
    [SerializeField] private float mPushDownForce;
    [SerializeField] private float mMinimumDamage;
    [SerializeField] private float mMaximumDamage;
    [SerializeField] [Range(0.0f, 1.0f)] private float mVelocityScaledDamageFactor;

    private List<GameObject> mAlreadyHit;
    private float mHighestMeasuredVelocity = 0;

    public override void Awake()
    {
        base.Awake();
        mExplosion.amplitude = explosionAmplitude;
    }


    void Update()
    {
        if (caster.movement.GetVelocity().y < mHighestMeasuredVelocity)
        {
            mHighestMeasuredVelocity = caster.movement.GetVelocity().y;
        }
    }

    public override void Execute(PlayerScript caster)
    {

        //get a fistoffury object
        FistOfFuryBehaviour fof = PoolRegistry.FistOfFuryPool.Get(Pool.Activation.ReturnActivated).GetComponent<FistOfFuryBehaviour>();
        fof.caster = caster;
        fof.transform.position = caster.transform.position;
        fof.transform.parent = caster.transform;
        fof.mAlreadyHit = new List<GameObject>();
        //spawn it on all clients
        NetworkServer.Spawn(fof.gameObject);


        //check whether caster is airborn or grounded
        if (!caster.movement.feet.IsGrounded())
        {
            //set caster's state so he or she doesnt get falldamage
            caster.SetEffectState(EffectStateSystem.EffectStateID.NoFallDamage);
            caster.movement.RpcAddForce(Vector3.down * mPushDownForce, ForceMode.VelocityChange);
        }
    }

    protected override void ExecuteTriggerEnter_Host(Collider collider)
    {
        if (collider.isTrigger) return;
        
        //spawn an explosion
        GameObject go = PoolRegistry.ExplosionPool.Get();
        go.transform.position = caster.transform.position/* + caster.transform.forward * 5*/;
        go.SetActive(true);
        Explosion ex = go.GetComponent<Explosion>();
        if (ex)
        {
            ex.amplitude = explosionAmplitude;
        }
        NetworkServer.Spawn(go);


        //Debug.Log("velocity: " + caster.movement.GetVelocity());

        //apply explosiondamage to all healthscripts that were found
        Collider[] colliders = Physics.OverlapSphere(caster.transform.position, explosionAmplitude);
        for (int i = 0; i < colliders.Length; ++i)
        {
            Rigidbody rigid = colliders[i].attachedRigidbody;
            if (rigid && !mAlreadyHit.Contains(rigid.gameObject))
            {
                mAlreadyHit.Add(rigid.gameObject);
                PlayerScript ps = colliders[i].attachedRigidbody.GetComponent<PlayerScript>();
                Vector3 direction = rigid.position - caster.movement.mRigidbody.position;
                if (ps)
                {
                    if (ps != caster)
                    {
                        ps.movement.RpcAddForce(mExplosionForce*direction, ForceMode.VelocityChange);
                    }
                }
                else
                {
                    rigid.AddForce(mExplosionForce*direction, ForceMode.VelocityChange);
                }

                HealthScript hs = rigid.GetComponentInParent<HealthScript>();
                if (hs && hs != caster.healthScript)
                {
                    if (mHighestMeasuredVelocity < 0) mHighestMeasuredVelocity *= -1;
                    float damage = mMinimumDamage + mHighestMeasuredVelocity*mVelocityScaledDamageFactor;
                    damage = Mathf.Clamp(damage, mMinimumDamage, mMaximumDamage);
                    Debug.Log("damage by fist: " + damage);
                    Debug.Log("highest vel: " + mHighestMeasuredVelocity);
                    hs.TakeDamage(damage);
                }
            }
        }

        //remove the fistoffury object on all clients
        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);

        //Set state of player to normal
        caster.RpcSetEffectState(EffectStateSystem.EffectStateID.Normal);
    }

    protected override void ExecuteCollision_Host(Collision collision) { }
}