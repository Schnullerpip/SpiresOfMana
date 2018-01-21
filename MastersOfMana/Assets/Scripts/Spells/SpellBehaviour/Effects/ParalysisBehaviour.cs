using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ParalysisBehaviour : A_EffectBehaviour
{
    [SerializeField] private float mHitRadius;
    [SerializeField] private float mHitRange;

    //reduction (start to crack when damaged etc)
    [SerializeField] private float mReductionIntervalInSeconds;
    [SerializeField] private float mLifeTime;
    private int mDamagePerSecond;
    [SerializeField] private float mLifetimeAfterShatter;
    [SerializeField] private GameObject mShatterEffect;
    [SerializeField] private Transform[] mFragmentTransforms;


    [SerializeField] private CapsuleCollider iceCollider;
    [SerializeField] private HealthScript healthscript;

    [SerializeField] public float DamageRemitFactor = 1.0f;

    //just a part of the effect that needs to be reset manually or it wont work with pooling
    [SerializeField] private GameObject ActivateOnDeactivation;

    //tracks the lifetime of the paralysis
    private float mTimeCount;
    //indicates whether or not to follow the affected Player
    private bool mFollowTarget;


    //damage representation and visual reduction
    //cache all the fragments
    [SerializeField] private GameObject[] fragments;

    //the effect original (to copy) whenever nothing was hit
    [SerializeField] private GameObject mNonHitEffect;
    [SerializeField] private GameObject mTakeDamageEffect;
    private ParticleSystem DamageEffect;

    [SyncVar] private GameObject mAffectedPlayerObject;
    private PlayerScript mAffectedPlayer;

	public override void Preview (PlayerScript caster)
	{
		base.Preview(caster);

        Vector3 hitPlayerPos;
        PlayerScript hitPlayer = HitAPlayer(caster, mHitRadius, mHitRange, out hitPlayerPos, false);

        if(hitPlayer)
        {
            preview.instance.Move(hitPlayerPos);
            return;
        }

        //hit geometry?
        caster.SetColliderIgnoreRaycast(true);
		Ray ray = caster.aim.GetCameraRig().GetCenterRay();
		RaycastHit hit;
        bool hitSomething = Physics.Raycast(ray, out hit);
        if(!hitSomething || hit.distance > mHitRange)
		{
            //we will hit nothing
            preview.instance.Deactivate();
		}
        else
        {
            preview.instance.Move(hit.point);
        }
        caster.SetColliderIgnoreRaycast(true);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();
	}

    public override void Execute(PlayerScript caster)
    {
        //check for a hit
        var opponents = GameManager.instance.players;
        foreach (var p in opponents)
        {
            if (p == caster) continue;

            if (ConfirmedHitServer(p.headJoint.position, caster, mHitRadius, mHitRange) ||
                ConfirmedHitServer(p.transform.position, caster, mHitRadius, mHitRange) ||
                ConfirmedHitServer(p.movement.mRigidbody.worldCenterOfMass, caster, mHitRadius, mHitRange))
            {
                //player was hit -> create an icecrystalsurrounding him/her
                ParalysisBehaviour pb = PoolRegistry.GetInstance(gameObject, p.transform.position, caster.transform.rotation, 2, 1) .GetComponent<ParalysisBehaviour>();

                pb.gameObject.layer = LayerMask.NameToLayer("FrostPrison");

                pb.Init(p.gameObject);
                pb.mAffectedPlayer = p;
                pb.caster = caster;
                pb.casterObject = caster.gameObject;

                NetworkServer.Spawn(pb.gameObject);

                p.SetEffectState(EffectStateSystem.EffectStateID.Frozen); //from now on only the ice crystal can damage the player

                return;
            }
        }

        // no player was hit - maybe geometry?
        {
            RaycastHit hit;
            caster.SetColliderIgnoreRaycast(true);
            bool hitSomething = Physics.Raycast(new Ray(caster.GetCameraPosition(), caster.GetCameraLookDirection()), out hit);
            caster.SetColliderIgnoreRaycast(false);
            if (!hitSomething || hit.distance > mHitRange) //not hitting anything
            {
                //we hit nothing in range -> spawn empty ice-ish explosion
                GameObject nonHitEffect = PoolRegistry.GetInstance(mNonHitEffect, 1, 1, Pool.PoolingStrategy.OnMissRoundRobin, Pool.Activation.ReturnActivated);
                nonHitEffect.transform.position = caster.GetCameraPosition() + caster.GetCameraLookDirection() * mHitRange;
                NetworkServer.Spawn(nonHitEffect);
            }
            else //hit some nonplayer object or terrain
            {
                if (hit.transform.gameObject.isStatic)
                {
                    //its definitely terrain
                    //get its normal and create an iceCrystal with the hit points normal as rotation
                    ParalysisBehaviour pb = PoolRegistry.GetInstance(gameObject, hit.point,Quaternion.FromToRotation(Vector3.up, hit.normal), 1, 1) .GetComponent<ParalysisBehaviour>();
                    pb.gameObject.layer = LayerMask.NameToLayer("Default");
                    pb.Init(null);
                    NetworkServer.Spawn(pb.gameObject);
                }
                else
                {
                    //we hit a nonterrain object - we would not want to spawn an icecrystal here (would just look strange to frost a flying grenade for example)
                    GameObject nonHitEffect = PoolRegistry.GetInstance(mNonHitEffect, hit.point, Quaternion.identity, 2, 2, Pool.PoolingStrategy.OnMissRoundRobin, Pool.Activation.ReturnActivated);
                    nonHitEffect.transform.position = hit.point;
                    NetworkServer.Spawn(nonHitEffect);
                }
            }
        }
    }

    #region Initialization
    private void Init(GameObject affectedPlayer)
    {
        gameObject.SetActive(true);
        mAffectedPlayerObject = affectedPlayer;
        mTimeCount = 0;
        ActivateOnDeactivation.SetActive(true);
        healthscript.ResetObject();

        //according to how long the paralysis should be applied, calculate how much damage the icecrystal has to inflict to itself per second to disappear after its lifetime
        mDamagePerSecond = (int)(healthscript.GetMaxHealth()/mLifeTime*mReductionIntervalInSeconds);

        //make sure all the fragments are inactive
        for (int i = 0; i < fragments.Length; ++i)
        {
            var frag = fragments[i];
            frag.SetActive(false);
            var rigid = frag.GetComponent<Rigidbody>();
            rigid.isKinematic = true;
            rigid.useGravity = false;

            //make sure the fragments are in place always -> copy their dummy twin's transform
            var fragTrans = frag.transform;
            var dummyTrans = mFragmentTransforms[i];
            fragTrans.position = dummyTrans.position;
            fragTrans.rotation = dummyTrans.rotation;
        }
    }

    public void OnEnable()
    {
        mAffectedPlayer = null;
        mShatterEffect.SetActive(false);
        mFollowTarget = true;
        mLastIndex = 0;
        iceCollider.isTrigger = false;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //slow down the affected Player
        if (mAffectedPlayerObject)
        {
            mAffectedPlayer = mAffectedPlayerObject.GetComponent<PlayerScript>();
            gameObject.layer = LayerMask.NameToLayer("FrostPrison");

            ApplyMaliciousEffect();
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    void Start()
    {
        DamageEffect = mTakeDamageEffect.GetComponent<ParticleSystem>();

        healthscript.OnDamageTaken += VisualCrackPerDamageUnit; //make the icecrystal crack per damage normalized unit
        healthscript.OnDamageTaken += VisualizeDamageTaken; //Spawn a little particle effect, that looks like shards falling off the ice
        if (isServer)
        {
            healthscript.OnDamageTaken += DisappearWhenDead;
            healthscript.OnDamageTaken += RemitDamage; //redirect the icecrystals damage to the player
        }
    }

    void OnDestroy()
    {
        healthscript.OnDamageTaken -= VisualizeDamageTaken;
        healthscript.OnDamageTaken -= VisualCrackPerDamageUnit;
        if (isServer)
        {
            healthscript.OnDamageTaken -= DisappearWhenDead;
            healthscript.OnDamageTaken -= RemitDamage;
        }
    }


    /// <summary>
    /// will make the affected player immovable, ignoring all input and also preventing being moved by the server
    /// </summary>
    private void ApplyMaliciousEffect()
    {
        if (mAffectedPlayer)
        {
            //clear movement input with player
            mAffectedPlayer.movement.ClearMovementInput();
            mAffectedPlayer.movement.SetMovementAllowed(false);

            mAffectedPlayer.GetPlayerSpells().StopPreview();
            mAffectedPlayer.inputStateSystem.current.SetPreview(false);

            //slow down/stop the affected player
            mAffectedPlayer.inputStateSystem.SetState(InputStateSystem.InputStateID.Paralyzed);
        }
    }
    #endregion

    #region UpdateRoutines
    public void Update()
    {
        mTimeCount += Time.deltaTime;

        if (isServer && mTimeCount >= mReductionIntervalInSeconds)
        {
            mTimeCount = 0;

            //damages itself, so it will disappear eventually
            mRemitDamage = false; //dont remit the damage to the affected player for self inflicted damage
            healthscript.TakeDamage(mDamagePerSecond, null, GetType());
            mRemitDamage = true; //all other damage sources should go through to the affected player
        }
    }

    public void FixedUpdate()
    {
        if (mAffectedPlayer)
        {
            if (mFollowTarget)
            {
                transform.position = mAffectedPlayer.transform.position;
            }
        }
    }
    #endregion

    #region EventReactions
    /// <summary>
    /// makes sure the ice crystal will disappear when it has taken enough damage
    /// </summary>
    /// <param name="amount"></param>
    private void DisappearWhenDead(int amount)
    {
        if (isServer && !healthscript.IsAlive())
        {
            RestoreNormalState();
            RpcDisappear();
            StartCoroutine(DisappearAfterSeconds(this, mLifetimeAfterShatter));
        }
    }

    /// <summary>
    /// puts the damage taken through to the affectedPlayer
    /// </summary>
    /// <param name="amount"></param>
    private bool mRemitDamage = true;
    private void RemitDamage(int amount) {
        if (mRemitDamage && mAffectedPlayer)
        {
            mAffectedPlayer.healthScript.TakeDamage((int)(amount * DamageRemitFactor), caster, GetType());
        }
    }
    /// <summary>
    /// will activate fragments so the ice crystal starts to 'crack' open depending on the damage taken
    /// </summary>
    /// <param name="amount"></param>
    private int mLastIndex;
    private void VisualCrackPerDamageUnit(int amount)
    {
        float rate = 1.1f - healthscript.GetCurrentHealthPercentage();
        //how many of the fragments should be activated? (instant cast to int)
        int numberActivatedFragments = (int)(rate*fragments.Length);

        //activate all the fragments until the rate matches the damage
        for (int i = mLastIndex; i < Mathf.Min(numberActivatedFragments, fragments.Length); ++i)
        {
            fragments[i].SetActive(true);
        }

        mLastIndex = numberActivatedFragments;
    }

    private void VisualizeDamageTaken(int amount)
    {
        DamageEffect.Emit(amount*10);
    }
    #endregion

    #region NormalStateRestoration
    private void RestoreNormalState()
    {
        if (mAffectedPlayer)
        {
            //revert back to normal status
            mAffectedPlayer.SetEffectState(EffectStateSystem.EffectStateID.Normal);
            mAffectedPlayer.movement.SetMovementAllowed(true);
            mAffectedPlayer.SetInputState(InputStateSystem.InputStateID.Normal);
        }
    }

    [ClientRpc]
    private void RpcDisappear()
    {
        Shatter();
    }

    /// <summary>
    /// makes the crystals fragments be affected by physics (gravity), so they fall apart
    /// </summary>
    private void Shatter()
    {
        mShatterEffect.SetActive(true);
        mFollowTarget = false;

        //disallow collisions for the icecrystal collider
        iceCollider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("IgnoreAll");

        //disable the actual crystal and replace it completely with the fragments
        ActivateOnDeactivation.SetActive(false);

        //each fragments rigid body should now be affected by gravity (also not kinematic anymore) 
        for (int i = 0; i < fragments.Length; ++i)
        {
            var rigid = fragments[i].GetComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.useGravity = true;
            rigid.AddExplosionForce(1000.0f, transform.position, 30.0f);
        }
    }

    IEnumerator DisappearAfterSeconds(ParalysisBehaviour pb, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        pb.gameObject.SetActive(false);

        if (isServer)
        {
            NetworkServer.UnSpawn(pb.gameObject);
        }
    }
    #endregion
}