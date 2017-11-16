﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class FistOfFuryBehaviour : A_SummoningBehaviour
{
    private PlayerScript caster;

    [SerializeField] private float explosionAmplitude;
    [SerializeField] private ParticleSystem mTrail;

    [SerializeField] private float mExplosionForce;
    [SerializeField] private float mPushDownForce;
    [SerializeField] private float mMinimumDamage;
    [SerializeField] private float mMaximumDamage;
    [SerializeField] [Range(0.0f, 1.0f)] private float mDamageFactor;
    [SerializeField] private GameObject explosionPrefab;

    private List<GameObject> mAlreadyHit;
    //will store the transform.position of the caster when he casted - the difference between that and he collisionpoint will be a factor to the resulting damage
    private Vector3 castPosition;

    public override void Execute(PlayerScript caster)
    {
        //get a fistoffury object
        //FistOfFuryBehaviour fof = PoolRegistry.FistOfFuryPool.Get(Pool.Activation.ReturnActivated).GetComponent<FistOfFuryBehaviour>();
        FistOfFuryBehaviour fof = PoolRegistry.instance.Instantiate(this.gameObject).GetComponent<FistOfFuryBehaviour>();
        fof.caster = caster;
        fof.transform.position = fof.castPosition = caster.transform.position;
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
        //GameObject explosion = PoolRegistry.ExplosionPool.Get();
        GameObject explosion = PoolRegistry.instance.Instantiate(explosionPrefab);
        explosion.transform.position = caster.transform.position/* + caster.transform.forward * 5*/;
        explosion.SetActive(true);
        Explosion ex = explosion.GetComponent<Explosion>();
        if (ex)
        {
            ex.amplitude = explosionAmplitude;
        }
        NetworkServer.Spawn(explosion);


        //Debug.Log("velocity: " + caster.movement.GetVelocity());

        //apply explosiondamage to all healthscripts that were found
        Collider[] colliders = Physics.OverlapSphere(caster.transform.position, explosionAmplitude);
        for (int i = 0; i < colliders.Length; ++i)
        {
            Rigidbody rigid = colliders[i].attachedRigidbody;
            if (rigid && !mAlreadyHit.Contains(rigid.gameObject))
            {
                mAlreadyHit.Add(rigid.gameObject);
                Vector3 direction = rigid.position - caster.movement.mRigidbody.position;
                direction.Normalize();

                //if the hit object is hurtable - hurt it
                float distance = Vector3.Magnitude(caster.transform.position - castPosition);
                float resultingDamage = mMinimumDamage + distance*mDamageFactor;
                HealthScript hs = rigid.GetComponentInParent<HealthScript>();//searches in the gameobject AND in its parents
                if (hs && hs != caster.healthScript)
                {
                    //calculate damage
                    resultingDamage = Mathf.Clamp(resultingDamage, mMinimumDamage, mMaximumDamage);
                    Debug.Log("damage by fist: " + resultingDamage);
                    hs.TakeDamage(resultingDamage);
                }

                //if the hit object is moveable - move it
                ServerMoveable sm = rigid.gameObject.GetComponent<ServerMoveable>();
                if (sm)
                {
                    if (sm.gameObject != caster.gameObject)
                    {
                        sm.RpcAddForce(mExplosionForce*direction, ForceMode.VelocityChange);
                    }
                }
                else
                {
                    rigid.AddForce(mExplosionForce*direction, ForceMode.VelocityChange);
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