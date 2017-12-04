using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StormBlastBehaviour : A_EffectBehaviour
{

    [SerializeField] private float mRadius = 10;
    [SerializeField] private float mForce = 10;
    [SerializeField] private float mInterval = 10;
    [SerializeField] private float mLifeTime = 10;
    [SerializeField] private ParticleSystem mBlastEffect;

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    // Update is called once per frame
    private float mTimeCount = 0;
	void Update () {

        //reposition the object
	    transform.position = caster.transform.position;

	    if ((mTimeCount += Time.deltaTime) > mInterval)
	    {
            //reset timer
            mTimeCount = 0;

            //activate the particle system
            mBlastEffect.Play();

	        if (isServer)
	        {
	            //push all servermoveables inside the radius away
	            foreach (var c in Physics.OverlapSphere(caster.transform.position, mRadius))
	            {
	                Rigidbody rigid = c.attachedRigidbody;
	                ServerMoveable sm;
	                //is collider a serverMoveable?
	                if (rigid && (sm = rigid.GetComponentInParent<ServerMoveable>()))
	                {
	                    //move it according to the force
	                    Vector3 velocity = (sm.transform.position - caster.transform.position).normalized*mForce;
	                    sm.RpcSetVelocity(velocity);
	                }
	            }
	        }
	    }
	}

    public override void Execute(PlayerScript caster)
    {
        StormBlastBehaviour sbb = PoolRegistry.Instantiate(this.gameObject).GetComponent<StormBlastBehaviour>();
        sbb.casterObject = caster.gameObject;
        sbb.caster = caster;
        sbb.mTimeCount = mInterval;

        sbb.gameObject.SetActive(true);
        NetworkServer.Spawn(sbb.gameObject);

        sbb.StartCoroutine(UnspawnStormblastAfterSeconds(sbb.gameObject, mLifeTime));
    }

    IEnumerator UnspawnStormblastAfterSeconds(GameObject go, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        go.SetActive(false);
        NetworkServer.UnSpawn(go);
    }
}