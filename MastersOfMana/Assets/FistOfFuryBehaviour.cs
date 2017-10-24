using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class FistOfFuryBehaviour : A_SummoningBehaviour
{

    [SerializeField] private float explosionAmplitude;
    private PlayerScript caster;
    [SerializeField]
    private Explosion mExplosion;
    [SerializeField]
    private ParticleSystem mTrail;

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
        if (!caster.feet.IsGrounded())
        {
            //set caster's state so he or she doesnt get falldamage
            caster.RpcSetEffectState(EffectStateSystem.EffectStateID.NoFallDamage);
            caster.RpcAddForce(Vector3.down * 40.0f, (int)ForceMode.VelocityChange);
        }
    }

    protected override void ExecuteTriggerEnter_Host(Collider collider)
    {
        //set state to superherolanding TODO

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

        //apply explosiondamage to all healthscripts that were found
        Collider[] colliders = Physics.OverlapSphere(caster.transform.position, explosionAmplitude);
        for (int i = 0; i < colliders.Length; ++i)
        {
            HealthScript hs = colliders[i].GetComponent<HealthScript>();
            if (hs && hs != caster.healthScript)
            {
                hs.TakeDamage(10.0f);
            }

            Rigidbody rigid = collider.attachedRigidbody;
            if (rigid)
            {
                PlayerScript ps = collider.attachedRigidbody.GetComponent<PlayerScript>();
                if (ps)
                {
                    ps.RpcAddExplosionForce(40.0f, caster.transform.position, explosionAmplitude);

                    print("explosionradius: " + explosionAmplitude/2);
                    print("curve: " + mExplosion.aCurve.Evaluate(1));
                    print("amplitude: " + explosionAmplitude);
                }
                else
                {
                    collider.attachedRigidbody.AddExplosionForce(40.0f, caster.transform.position, explosionAmplitude);
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