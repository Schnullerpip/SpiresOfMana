using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireballBehaviour : A_SpellBehaviour
{
    private Rigidbody mRigid;

    public void Start()
    {
        mRigid = GetComponent<Rigidbody>();
        if (!mRigid)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException();
        }
    }

    public override void Execute(PlayerScript caster)
    {
        GameObject fireball = PoolRegistry.FireballPool.Get(Pool.Activation.ReturnActivated);
        fireball.transform.position = caster.handTransform.position + caster.transform.forward * 0.5f;
        fireball.GetComponent<Rigidbody>().velocity = caster.transform.forward*5.0f;//TODO exchange magic numbers with something relevant
    }
}
