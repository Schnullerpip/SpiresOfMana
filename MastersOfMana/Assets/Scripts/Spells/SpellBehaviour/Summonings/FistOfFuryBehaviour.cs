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

    public override void Awake()
    {
        base.Awake();
        mExplosion.amplitude = explosionAmplitude;
    }

    public override void Execute(PlayerScript caster)
    {

        //get a fistoffury object
        FistOfFuryBehaviour fof = PoolRegistry.FistOfFuryPool.Get(Pool.Activation.ReturnActivated).GetComponent<FistOfFuryBehaviour>();
        fof.caster = caster;
        fof.transform.position = caster.transform.position;
        fof.transform.parent = caster.transform;
        //spawn it on all clients
        NetworkServer.Spawn(fof.gameObject);


        //check whether caster is airborn or grounded
        if (!caster.movement.feet.IsGrounded())
        {
            //set caster's state so he or she doesnt get falldamage
            caster.RpcSetEffectState(EffectStateSystem.EffectStateID.NoFallDamage);
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
            if (rigid)
            {
                HealthScript hs = rigid.GetComponent<HealthScript>();
                if (hs && hs != caster.healthScript)
                {
                    float damage = mMinimumDamage + caster.movement.GetVelocity().magnitude*mVelocityScaledDamageFactor;
                    damage = Mathf.Clamp(damage, mMinimumDamage, mMaximumDamage);
                    Debug.Log("damage dealt: " + damage);
                    hs.TakeDamage(damage);
                }

                //TODO exchange magic numbers with good stuff... 
                PlayerScript ps = colliders[i].attachedRigidbody.GetComponent<PlayerScript>();
                if (ps)
                {
                    if (ps != caster)
                    {
                        ps.movement.RpcAddExplosionForce(mExplosionForce, caster.transform.position, explosionAmplitude);
                    }
                }
                else
                {
                    rigid.AddExplosionForce(mExplosionForce, caster.transform.position, explosionAmplitude);
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