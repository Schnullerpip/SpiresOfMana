using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WindWallBehaviour : A_SummoningBehaviour
{
    [SerializeField]
    private Vector3 mWindForceDirection;

    [SerializeField]
    private float mWindforceStrength;

    [SerializeField]
    private float mCenterDistance;

    private Vector3 force;
    private Vector3 center;
    private PlayerScript caster;

    //prevent players from getting forces of the windwall for each collider they have
    private List<GameObject> mAlreadyAffected;

    public void OnEnable()
    {
        mAlreadyAffected = new List<GameObject>();
    }

    public void OnValidate()
    {
        mWindForceDirection.Normalize();
    }

    public override void Execute(PlayerScript caster)
    {
        GameObject ww = PoolRegistry.WindWallPool.Get();
        WindWallBehaviour windwall = ww.GetComponent<WindWallBehaviour>();

		Vector3 direction =	GetAim(caster);

        //put the center of the windbox infront of the caster
		windwall.center = caster.handTransform.position + direction * mCenterDistance;

		windwall.force = mWindforceStrength*(caster.transform.rotation*mWindForceDirection);

        windwall.transform.rotation = caster.aim.currentLookRotation;

        windwall.transform.position = windwall.center;
        windwall.caster = caster;

        Rigidbody rigid = ww.GetComponent<Rigidbody>();
		rigid.velocity = caster.movement.GetVelocity();

        ww.SetActive(true);
        NetworkServer.Spawn(ww);

        //make sure to unspawn the windwall
        caster.StartCoroutine(UnspawnWindwall(ww));
    }

    IEnumerator UnspawnWindwall(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(false);
        NetworkServer.UnSpawn(obj);
    }

    protected override void ExecuteCollision_Host(Collision collision) { }

    protected override void ExecuteTriggerEnter_Host(Collider other)
    { 
        Rigidbody rigid = other.attachedRigidbody;

        if(rigid && !mAlreadyAffected.Contains(other.gameObject))
        {
            mAlreadyAffected.Add(other.gameObject);
            PlayerScript opponent = rigid.GetComponent<PlayerScript>();
            
            if (opponent)
            {
                if (opponent != caster )
                {
                    opponent.movement.RpcSetVelocity(force);
                }
            }
            else
            {
                rigid.velocity = force;
            }	
        }
    }
}