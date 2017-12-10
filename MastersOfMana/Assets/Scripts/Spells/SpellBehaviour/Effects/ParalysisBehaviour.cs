using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ParalysisBehaviour : A_EffectBehaviour
{
    [SerializeField] private float mLifetime;
    [SerializeField] private ParticleSystem mParticleSystem;
    [SerializeField] private float mRange;

    private float mTimeCount;

    [SyncVar] private GameObject mAffectedPlayerObject;
    private PlayerScript mAffectedPlayer;

    public override void Execute(PlayerScript caster)
    {
        //check for a hit
        var opponents = GameManager.instance.mPlayers;
        foreach (var p in opponents)
        {
            if (p == caster) continue;

            if (ConfirmedHit(p.headJoint.position, caster, mRange) ||
                ConfirmedHit(p.transform.position, caster, mRange) ||
                ConfirmedHit(p.movement.mRigidbody.worldCenterOfMass, caster, mRange))
            {
                //create a Paralysis behaviour, that sticks to the hitPlayer
                ParalysisBehaviour pb = PoolRegistry.GetInstance(this.gameObject, 4, 4).GetComponent<ParalysisBehaviour>();
                pb.mAffectedPlayerObject = p.gameObject;
                pb.gameObject.SetActive(true);
                pb.mTimeCount = 0;
                NetworkServer.Spawn(pb.gameObject);
                //apply damage just so the system registeres it as an affect
                p.healthScript.TakeDamage(0, GetType());
                return;
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
        //clear movement input with player
        mAffectedPlayer.movement.ClearMovementInput();
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