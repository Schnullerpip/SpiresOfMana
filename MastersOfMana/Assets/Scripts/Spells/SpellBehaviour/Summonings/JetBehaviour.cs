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
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float mMinimalHeightFactor;

    [SerializeField] private CapsuleCollider trigger;
    private Vector3 triggerPos;

    private List<ServerMoveable> mAffecting = null;

    public override void Execute(PlayerScript caster)
    {
        RaycastHit hit;
        if (Physics.Raycast( new Ray(caster.movement.mRigidbody.worldCenterOfMass + caster.transform.forward * mOffsetToCaster, Vector3.down), out hit, 100))
        {
            //GameObject jet = PoolRegistry.JetPool.Get();
            JetBehaviour jet = PoolRegistry.GetInstance(this.gameObject, 4, 4).GetComponent<JetBehaviour>();
            jet.transform.position = hit.point;
            jet.caster = caster;
            jet.casterObject = caster.gameObject;
            jet.triggerPos = jet.trigger.transform.position;
            jet.triggerPos = new Vector3(0, jet.triggerPos.y, 0);

            jet.gameObject.SetActive(true);
            NetworkServer.Spawn(jet.gameObject);

            caster.StartCoroutine(UnSpawnJetAfterSeconds(jet.gameObject, mLifeTime));
        }
    }

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

        preview.SetAvailability(caster.CurrentSpellReady());
        preview.instance.MoveAndRotate(caster.transform.position + GetAimClient(caster) * mOffsetToCaster, Quaternion.identity);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();
	}

    void Start()
    {
        if (mAffecting == null)
        {
            mAffecting = new List<ServerMoveable>();
        }
    }

    void OnEnable()
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
            //Affecting the caster should happen locally, or stutters will occure
            if (sm  && !mAffecting.Contains(sm))
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

    public void FixedUpdate()
    {
        if (!isServer || !caster)
        {
            return;
        }

        Vector3 upforce = Vector3.up * mUpForce;

        foreach (var sm in mAffecting)
        {
            Vector3 upforceIndividual = upforce;
            //we only wanna deal with the 
            Vector3 smPos = sm.transform.position;
            smPos.x = 0;
            smPos.z = 0;

            float factor = mMinimalHeightFactor + 1 - (triggerPos - smPos).sqrMagnitude / (trigger.height * trigger.height);
            factor *= factor;
            upforceIndividual *= factor;

            if (sm.mRigidbody.velocity.y < 0)
            {
                Vector3 damping = -1 * sm.mRigidbody.velocity;
                damping.x = 0;
                damping.z = 0;
                damping *= mGravityreduction;
                upforceIndividual += damping;
            }

            sm.RpcAddForce(upforceIndividual, ForceMode.Force);
        }
    }

    IEnumerator UnSpawnJetAfterSeconds(GameObject jet, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        jet.SetActive(false);
        NetworkServer.UnSpawn(jet);
    }
}