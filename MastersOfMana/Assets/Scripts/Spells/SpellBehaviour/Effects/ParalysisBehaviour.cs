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

    [SyncVar] private GameObject mAffectedPlayerObject;
    private PlayerScript mAffectedPlayer;

	public override void Preview (PlayerScript caster)
	{
		base.Preview(caster);

        preview.instance.SetAvailability(caster.CurrentSpellReady());

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
        if(Physics.Raycast(ray, out hit))
		{
			preview.instance.Move(hit.point); 
		}
		else
		{
            preview.instance.Deactivate();
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
                //create an icecrystal
                ParalysisBehaviour pb = PoolRegistry.GetInstance(gameObject, p.transform.position, caster.transform.rotation, 1, 1) .GetComponent<ParalysisBehaviour>();

                pb.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");

                pb.Init(p.gameObject);
                pb.mAffectedPlayer = p;

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
            if (hitSomething)
            {
                //whatever it is its not a player - get its normal and create an iceCrystal with the hit points normal as rotation
                ParalysisBehaviour pb = PoolRegistry.GetInstance(gameObject, hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal), 1, 1).GetComponent<ParalysisBehaviour>();
                pb.gameObject.layer = LayerMask.NameToLayer("Default");
                pb.Init(null);
                NetworkServer.Spawn(pb.gameObject);
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

    void Start()
    {
        healthscript.OnDamageTaken += VisualCrackPerDamageUnit; //make the icecrystal crack per damage normalized unit
        if (isServer)
        {
            healthscript.OnDamageTaken += DisappearWhenDead;
            healthscript.OnDamageTaken += RemitDamage; //redirect the icecrystals damage to the player
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
            healthscript.TakeDamage(mDamagePerSecond, GetType());
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
            //prevent stair sliding
            if (mAffectedPlayer.movement.feet.IsGrounded())
            {
                mAffectedPlayer.movement.mRigidbody.velocity = Vector3.zero;
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
            mAffectedPlayer.healthScript.TakeDamage((int)(amount * DamageRemitFactor), GetType());
        }
    }
    /// <summary>
    /// will activate fragments so the ice crystal starts to 'crack' open depending on the damage taken
    /// </summary>
    /// <param name="amount"></param>
    private int mLastIndex;
    private void VisualCrackPerDamageUnit(int amount)
    {
        float rate = 1.1f - healthscript.GetCurrentHealth()*1.0f/healthscript.GetMaxHealth();
        //how many of the fragments should be activated? (instant cast to int)
        int numberActivatedFragments = (int)(rate*fragments.Length);

        //activate all the fragments until the rate matches the damage
        for (int i = mLastIndex; i < Mathf.Min(numberActivatedFragments, fragments.Length); ++i)
        {
            fragments[i].SetActive(true);
        }

        mLastIndex = numberActivatedFragments;
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
        gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");

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