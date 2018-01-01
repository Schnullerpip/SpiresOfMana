using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class StormBlastBehaviour : A_EffectBehaviour
{

    [SerializeField] private float mRadius = 10;
    [SerializeField] private float mForce = 10;
    [SerializeField] private float mInterval = 10;
    [SerializeField] private float mLifeTime = 10;
    [SerializeField] private ParticleSystem mBlastEffect;
    private bool mIsActive = false;

    void OnEnable()
    {
        mIsActive = true;
    }

    // Update is called once per frame
    private float mTimeCount = 0;
	void Update () {

        //reposition the object
	    transform.position = caster.transform.position;

	    if (mIsActive && ((mTimeCount += Time.deltaTime) > mInterval))
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

                        //if its a player also affect it (for stats)
                        PlayerScript ps;
                        if (ps = rigid.GetComponentInParent<PlayerScript>())
                        {
                            ps.healthScript.TakeDamage(0, GetType());
                        }
	                }
	            }
	        }
	    }
	}

    public override void Execute(PlayerScript caster)
    {
        StormBlastBehaviour sbb = PoolRegistry.GetInstance(gameObject, 1, 1).GetComponent<StormBlastBehaviour>();
        sbb.casterObject = caster.gameObject;
        sbb.caster = caster;
        sbb.mTimeCount = mInterval;

        sbb.gameObject.SetActive(true);
        NetworkServer.Spawn(sbb.gameObject);

        GameManager.instance.RegisterUltiSpell(sbb);
        sbb.StartCoroutine(UnspawnStormblastAfterSeconds(sbb, mLifeTime));
        caster.healthScript.OnInstanceDied += sbb.EndSpell;
    }

    public override void EndSpell()
    {
        caster.healthScript.OnInstanceDied -= EndSpell;
        mIsActive = false;
        gameObject.SetActive(false);
        GameManager.instance.UnregisterUltiSpell(this);
        NetworkServer.UnSpawn(gameObject);
    }

    IEnumerator UnspawnStormblastAfterSeconds(StormBlastBehaviour sbb, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (sbb.gameObject.activeSelf && mIsActive)
        {
            sbb.EndSpell();
        }
    }
}