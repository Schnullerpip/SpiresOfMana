using System;
using UnityEngine;
using UnityEngine.Networking;

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

    public override void Awake()
    {
        base.Awake();
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

        //now activate it, so no weird interpolation errors occcure
        //TODO delete this eventually - RPCs are just too slow
        //fireball.GetComponent<A_SummoningBehaviour>().RpcSetActive(true);
        fireball.SetActive(true);

        //speed up the fireball to fly into the lookdirection of the player
        FireballBehaviour fb = fireball.GetComponent<FireballBehaviour>();
        fb.mRigid.velocity = caster.GetAimDirection() * mSpeed;

        //create an instance of this fireball on the client's machine
        NetworkServer.Spawn(fireball, PoolRegistry.FireballPool.assetID);
    }

    protected override void ExecuteCollision_Host(Collision collision) {
        HealthScript hs = collision.gameObject.GetComponent<HealthScript>();
        if (hs)
        {
            hs.TakeDamage(mDamage);
        }

        PreventInterpolationIssues();
        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);
    }
}