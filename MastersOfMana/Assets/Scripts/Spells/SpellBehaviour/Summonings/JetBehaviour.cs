using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JetBehaviour : A_SummoningBehaviour
{

    [SerializeField]
    private float mOffsetToCaster;
    [SerializeField]
    private float mUpForce;
    [SerializeField]
    private float mLifeTime;
    [SerializeField]
    private float mGravityreduction;

    [SerializeField] private CapsuleCollider trigger;

    private List<ServerMoveable> mAffecting;


    public override void Execute(PlayerScript caster)
    {
        RaycastHit hit;
        if (Physics.Raycast( new Ray(caster.movement.mRigidbody.worldCenterOfMass + caster.transform.forward * mOffsetToCaster, Vector3.down), out hit, 100))
        {
            //GameObject jet = PoolRegistry.JetPool.Get();
            JetBehaviour jet = PoolRegistry.Instantiate(this.gameObject).GetComponent<JetBehaviour>();
            jet.transform.position = hit.point;
            jet.caster = caster;
            jet.casterObject = caster.gameObject;

            jet.gameObject.SetActive(true);
            NetworkServer.Spawn(jet.gameObject);

            caster.StartCoroutine(UnSpawnJetAfterSeconds(jet.gameObject, mLifeTime));
        }
    }

    void Start()
    {
        mAffecting = new List<ServerMoveable>();
    }

    protected override void ExecuteTriggerEnter_Host(Collider other)
    {
        Rigidbody rigid = other.attachedRigidbody;
        if (rigid)
        {
            //if we're colliding with a projectile from our own caster, dont affect it
            A_SummoningBehaviour summoning = rigid.GetComponentInParent<A_SummoningBehaviour>();
            if (summoning && summoning.GetCaster() == caster)
            {
                return;
            }

            ServerMoveable sm = other.attachedRigidbody.GetComponent<ServerMoveable>();
            if (sm && !mAffecting.Contains(sm))
            {
                mAffecting.Add(sm);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isServer)
        {
            return;
        }

        Rigidbody rigid = other.attachedRigidbody;
        if (rigid)
        {
            ServerMoveable sm = other.attachedRigidbody.GetComponent<ServerMoveable>();
            if (sm && mAffecting.Contains(sm))
            {
                mAffecting.Remove(sm);
            }
        }

    }

    public void Update()
    {
        if (!isServer || !caster)
        {
            return;
        }

        Vector3 upforce = Vector3.up*mUpForce;

        foreach (var sm in mAffecting)
        {
            if (sm.mRigidbody.velocity.y < 0)
            {
                Vector3 damping = -1*sm.mRigidbody.velocity;
                damping.x = 0;
                damping.z = 0;
                damping *= mGravityreduction;
                upforce += damping;
            }

            sm.RpcAddForce(upforce, ForceMode.Force);
        }
    }

    IEnumerator UnSpawnJetAfterSeconds(GameObject jet, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        jet.SetActive(false);
        NetworkServer.UnSpawn(jet);
    }
}