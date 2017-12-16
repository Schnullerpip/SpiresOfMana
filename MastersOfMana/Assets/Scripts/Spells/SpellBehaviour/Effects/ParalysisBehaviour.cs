using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Utility;

public class ParalysisBehaviour : A_EffectBehaviour
{
    [SerializeField] private float mLifetime;
    [SerializeField] private float mHitRadius;
    [SerializeField] private float mHitRange;
    [SerializeField] private CapsuleCollider iceCollider;
    [SerializeField] private HealthScript healthscript;

    //just a part of the effect that needs to be reset manually or it wont work with pooling
    [SerializeField] private GameObject ActivateOnDeactivation;

    //tracks the lifetime of the paralysis
    private float mTimeCount;
    //indicates whether or not to follow the affected Player
    private bool mFollowTarget;

    [SyncVar] private GameObject mAffectedPlayerObject;
    private PlayerScript mAffectedPlayer;

    public override void Execute(PlayerScript caster)
    {
        //check for a hit
        var opponents = GameManager.instance.players;
        foreach (var p in opponents)
        {
            if (p == caster) continue;

            if (ConfirmedHit(p.headJoint.position, caster, mHitRadius, mHitRange) ||
                ConfirmedHit(p.transform.position, caster, mHitRadius, mHitRange) ||
                ConfirmedHit(p.movement.mRigidbody.worldCenterOfMass, caster, mHitRadius, mHitRange))
            {
                //create an icecrystal
                ParalysisBehaviour pb = PoolRegistry.GetInstance(gameObject, p.transform.position, caster.transform.rotation, 4, 4) .GetComponent<ParalysisBehaviour>();
                pb.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
                pb.Init(p.gameObject);
                NetworkServer.Spawn(pb.gameObject);
                //apply damage just so the system registeres it as an affect
                p.healthScript.TakeDamage(0, GetType());

                return;
            }
        }

        // no player was hit - mayby geometry?
        {
            RaycastHit hit;
            caster.SetColliderIgnoreRaycast(true);
            bool hitSomething = Physics.Raycast(new Ray(caster.GetCameraPosition(), caster.GetCameraLookDirection()), out hit);
            caster.SetColliderIgnoreRaycast(false);
            if (hitSomething)
            {
                //whatever it is its not a player - get its normal and create an iceCrystal with the hit points normal as rotation
                ParalysisBehaviour pb = PoolRegistry.GetInstance(gameObject, hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal), 4, 4).GetComponent<ParalysisBehaviour>();
                pb.gameObject.layer = LayerMask.NameToLayer("Default");
                pb.Init(null);
                NetworkServer.Spawn(pb.gameObject);
            }
        }
    }

    private void Init(GameObject affectedPlayer)
    {
        gameObject.SetActive(true);
        mAffectedPlayerObject = affectedPlayer;
        mTimeCount = 0;
        ActivateOnDeactivation.SetActive(true);
        healthscript.OnDamageTaken += damage =>
        {
            if (!healthscript.IsAlive())
            {
                RpcDisappear();
            }
        };
    }


    public void OnEnable()
    {
        mFollowTarget = true;
        healthscript.ResetObject();
    }

    public void Update()
    {
        mTimeCount += Time.deltaTime;
        if (mFollowTarget && (mTimeCount >= mLifetime-1.5f))
        {
            mFollowTarget = false;
        }else if (mTimeCount >= mLifetime)
        {
            mTimeCount = 0;
            if (isServer)
            {
                RpcDisappear();
            }
        }
    }

    public void FixedUpdate()
    {
        if (mAffectedPlayer && mFollowTarget)
        {
            transform.position = mAffectedPlayer.transform.position;
        }
    }

    public override void OnStartClient()
    {
        //slow down the affected Player
        if (mAffectedPlayerObject)
        {
            mAffectedPlayer = mAffectedPlayerObject.GetComponent<PlayerScript>();
            //mAffectedPlayer.StartCoroutine(AffectPlayer());
            ApplyMaliciousEffect();
        }
    }

    private void ApplyMaliciousEffect()
    {
        if (mAffectedPlayer)
        {
            //clear movement input with player
            mAffectedPlayer.movement.ClearMovementInput();
            mAffectedPlayer.movement.SetMovementAllowed(false);
            //slow down/stop the affected player
            //mAffectedPlayer.movement.speed = mAffectedPlayer.movement.originalSpeed*mSlowFactor;
            mAffectedPlayer.inputStateSystem.SetState(InputStateSystem.InputStateID.Paralyzed);
        }
    }

    private void RestoreNormalState()
    {
        if (mAffectedPlayer)
        {
            //revert back to normal status
            mAffectedPlayer.movement.SetMovementAllowed(true);
            mAffectedPlayer.inputStateSystem.SetState(InputStateSystem.InputStateID.Normal);
        }
    }

    //implements the handshake between server and client - bcs we MUST guarantee, that normal state is restored before unspawning!!
    [ClientRpc]
    private void RpcDisappear()
    {
        RestoreNormalState();
        gameObject.SetActive(false);
        CmdUnspawnObject();
    }

    [Command]
    private void CmdUnspawnObject()
    {
        NetworkServer.UnSpawn(gameObject);
    }
}