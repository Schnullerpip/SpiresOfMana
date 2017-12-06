using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ParalysisBehaviour : A_EffectBehaviour
{
    [SerializeField] private float mMaxReach;
    [SerializeField] private float mLifetime;
    [SerializeField] private ParticleSystem mParticleSystem;

    private float mTimeCount;

    [SyncVar] private GameObject mAffectedPlayerObject;
    private PlayerScript mAffectedPlayer;

    public override void Execute(PlayerScript caster)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(caster.GetCameraPosition(), caster.GetCameraLookDirection()), out hit,
            mMaxReach))
        {
            Rigidbody rigid = hit.rigidbody;
            if (rigid)
            {
                PlayerScript hitPlayer = rigid.GetComponentInParent<PlayerScript>();
                if (hitPlayer)
                {
                    //create a Paralysis behaviour, that sticks to the hitPlayer
                    ParalysisBehaviour pb = PoolRegistry.Instantiate(this.gameObject).GetComponent<ParalysisBehaviour>();
                    pb.mAffectedPlayerObject = hitPlayer.gameObject;
                    pb.gameObject.SetActive(true);
                    NetworkServer.Spawn(pb.gameObject);
                    //apply damage just so the system registeres it as an affect
                    hitPlayer.healthScript.TakeDamage(0, GetType());
                }
            }
        }
    }

    void Start()
    {
        mTimeCount = 0;
        mParticleSystem.Play();
    }

    public override void OnStartClient()
    {
        //slow down the affected Player
        mAffectedPlayer = mAffectedPlayerObject.GetComponent<PlayerScript>();
        mAffectedPlayer.StartCoroutine(AffectPlayer());
    }

    private IEnumerator AffectPlayer()
    {
        //slow down/stop the affected player
        //mAffectedPlayer.movement.speed = mAffectedPlayer.movement.originalSpeed*mSlowFactor;
        mAffectedPlayer.inputStateSystem.SetState(InputStateSystem.InputStateID.Paralyzed);
        yield return new WaitForSeconds(mLifetime);
        //revert back to normal status
        mAffectedPlayer.inputStateSystem.SetState(InputStateSystem.InputStateID.Normal);
        //mAffectedPlayer.movement.speed = mAffectedPlayer.movement.originalSpeed;
    }



    void Update()
    {
        //Unspawn after mLifetime - on server!
        if ((mTimeCount += Time.deltaTime) > mLifetime)
        {
            if (isServer)
            {
                //let the effect disappear
                gameObject.SetActive(false);
                mParticleSystem.Stop();
                NetworkServer.UnSpawn(this.gameObject);
            }
        }
    }

    void LateUpdate()
    {
        //this should both happen on client and server

        //reposition
        transform.position = mAffectedPlayerObject.transform.position;
    }

}