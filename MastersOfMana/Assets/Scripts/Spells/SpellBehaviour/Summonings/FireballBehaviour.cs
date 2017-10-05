using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireballBehaviour : A_SummoningBehaviour
{
    private Rigidbody mRigid;
    [SerializeField]
    private float mSpeed = 5.0f;
    [SerializeField]
    private float mDamage = 5.0f;

    public override void Start()
    {
        base.Start();
        mRigid = GetComponent<Rigidbody>();
        if (!mRigid)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException();
        }
    }

    public override void Execute(PlayerScript caster)
    {
        //Get a fireballinstance out of the pool
        GameObject fireball = PoolRegistry.FireballPool.Get(Pool.Activation.ReturnActivated);

        //position the fireball to 'spawn' at the casters hand, including an offset so it does not collide instantly with the hand
        fireball.transform.position = caster.handTransform.position + caster.lookDirection * 1.5f;
        fireball.transform.rotation = caster.transform.rotation;

        //speed up the fireball to fly into the lookdirection of the player
        RaycastHit hit;
        if (caster.cameraRig.CenterRaycast(out hit))
        {
            fireball.GetComponent<Rigidbody>().velocity = Vector3.Normalize(hit.point - fireball.transform.position)*mSpeed;
        }
        else
        {
            fireball.GetComponent<Rigidbody>().velocity = caster.lookDirection*mSpeed;
        }
    }

    protected override void ExecuteCollisionOnServer(Collision collision) {
        RpcSetActive(false);
        HealthScript hs = collision.gameObject.GetComponent<HealthScript>();
        if (hs) {
            hs.TakeDamage(mDamage);
        }
    }
}
