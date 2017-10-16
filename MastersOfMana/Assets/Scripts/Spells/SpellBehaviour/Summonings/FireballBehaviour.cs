using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The specific behaviour of the fireball, that is manifested in the scene
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FireballBehaviour : A_SummoningBehaviour
{
    [SerializeField]
    private float mSpeed = 5.0f;
    [SerializeField]
    private float mDamage = 5.0f;

    public override void Start()
    {
        base.Start();
        if (!mRigid)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException();
        }
    }

    public override void Execute(PlayerScript caster)
    {
        //Get a fireballinstance out of the pool
        GameObject fireball = PoolRegistry.FireballPool.Get();

        //position the fireball to 'spawn' at the casters hand, including an offset so it does not collide instantly with the hand
        fireball.transform.position = caster.handTransform.position + caster.GetAimDirection() * 1.5f;
        fireball.transform.rotation = caster.transform.rotation;

        //now activate it, so no weird interpolation errors occcur
        fireball.GetComponent<A_SummoningBehaviour>().RpcSetActive(true);

        //speed up the fireball to fly into the lookdirection of the player
        fireball.GetComponent<Rigidbody>().velocity = caster.GetAimDirection() * mSpeed;
    }

    protected override void ExecuteCollisionOnServer(Collision collision) {
        HealthScript hs = collision.gameObject.GetComponent<HealthScript>();
        if (hs)
        {
            hs.TakeDamage(mDamage);
        }

        RpcPreventInterpolationIssues();
        RpcSetActive(false);
    }
}
