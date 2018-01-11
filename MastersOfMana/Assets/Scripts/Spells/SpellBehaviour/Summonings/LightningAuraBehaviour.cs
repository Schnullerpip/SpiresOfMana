using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LightningAuraBehaviour : A_SummoningBehaviour
{
    [SerializeField] private float mDuration = 5;
    [SerializeField] private float mCountTilDamage = 1.0f;
    [SerializeField] private int mDamage = 1;
    [SerializeField] private bool mInflictSelfDamage = false;
    private float mEnergyLevel;

    [SerializeField]
    private ParticleSystem mStaticPotential;
    [SerializeField]
    private ParticleSystem mLightningProjectile;

    public AudioSource mAudioSource;
    public AudioClip CastEffectClip;
    public AudioClip StaticEnergyLoop;
    public AudioClip AttackSound;


    //the list that captures which players are shot
    private List<HealthScript> mAlreadyCaught;

    [SerializeField] private SphereCollider mDecectionTrigger; 

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

	    preview.instance.transform.localScale = (mDecectionTrigger.radius * 2) * Vector3.one;
        preview.instance.Move(caster.movement.mRigidbody.worldCenterOfMass);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();
	}

    void OnEnable()
    {
        mAudioSource.PlayOneShot(CastEffectClip);
        mAlreadyCaught = new List<HealthScript>();
        mEnergyLevel = mDuration;
        mTimeCount = mCountTilDamage;
        mSelfInflictDamageTimeCount = 0;
    }

    public override void Execute(PlayerScript caster)
    {
        LightningAuraBehaviour la = PoolRegistry.GetInstance(this.gameObject, 4, 4).GetComponent<LightningAuraBehaviour>();
        la.transform.position = caster.transform.position;
        la.gameObject.SetActive(true);
        la.transform.rotation = Quaternion.Euler(Vector3.zero);
        la.caster = caster;
        la.casterObject = caster.gameObject;
        la.mEnergyLevel = mDuration;//should work in Start also
        NetworkServer.Spawn(la.gameObject);
    }

    public void OnTriggerStay(Collider other)
    {
        HealthScript hs = other.GetComponentInParent<HealthScript>();
        if (hs && hs != caster.healthScript)
        {
            PlayerScript ps = other.GetComponentInParent<PlayerScript>();

            Vector3 position, direction;
            position = caster.movement.mRigidbody.worldCenterOfMass;
            if (ps)
            {
                direction = ps.movement.mRigidbody.worldCenterOfMass - caster.movement.mRigidbody.worldCenterOfMass;
            }
            else
            {
                direction = hs.transform.position - caster.movement.mRigidbody.worldCenterOfMass;
            }
            direction.Normalize();

            RaycastHit hit;

            if (Physics.Raycast( new Ray(position, direction.normalized), out hit))
            {
                if (hit.collider == other)
                {
                    if (!mAlreadyCaught.Contains(hs))
                    {
                        mAlreadyCaught.Add(hs);

                        HealthScript.Died hsOnOnInstanceDied = null;
                        hsOnOnInstanceDied = () =>
                        {
                            RemoveInstanceFromList(hs);
                            hs.OnInstanceDied -= hsOnOnInstanceDied;
                        };
                        hs.OnInstanceDied += hsOnOnInstanceDied;
                    }
                }
            }
            else
            {
                mAlreadyCaught.Remove(hs);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        HealthScript hs = other.GetComponentInParent<HealthScript>();
        RemoveInstanceFromList(hs);
    }

    private void RemoveInstanceFromList(HealthScript hs)
    {
        if (hs)
        {
            mAlreadyCaught.Remove(hs);
            if (mAlreadyCaught.Count == 0)
            {
                ActivateStaticPotential();
            }
        }
    }

    void FixedUpdate()
    {
        if (!caster)
        {
            gameObject.SetActive(false);
            NetworkServer.UnSpawn(gameObject);
            return;
        }
        //reposition
        transform.position = caster.movement.mRigidbody.worldCenterOfMass;
    }

    private float mTimeCount, mSelfInflictDamageTimeCount;
    void Update()
    {
        //as long as we dont have a caster, do nothing
        if (!caster)
        {
            return;
        }

        //damage the caster as a penalty
        if(mInflictSelfDamage && isServer && (mSelfInflictDamageTimeCount += Time.deltaTime) >= 2.0f)
        {
            mSelfInflictDamageTimeCount = 0;
            caster.healthScript.TakeDamage(1, GetType());
        }

        if (mAlreadyCaught.Count == 0)
        {
            if (!mStaticPotential.isPlaying)
            {
                ActivateStaticPotential();
            }
        }
        else
        {
            if (!mLightningProjectile.isPlaying)
            {
                ActivateLightningProjectile();
            }
            //which opponent is nearest?
            HealthScript nearest = null;
            float distance = 100000.0f;

            //if there are more than one opponents in reach, choose the nearest
            for (int i = 0; i < mAlreadyCaught.Count; ++i)
            {
                var hs = mAlreadyCaught[i];
                if (hs == null)
                {
                    continue;
                }
                  
                float dist = Vector3.Distance(hs.transform.position, caster.transform.position);
                if (dist < distance)
                {
                    nearest = hs;
                }
            }

            if (nearest)
            {
                //since mAlreadyCaught is not empty we assume, that mLightningProjectile is already active
                //shoot the lightning projectile at nearest opponent
                ServerMoveable sm = nearest.GetComponent<ServerMoveable>();
                if (sm)
                {
                    mLightningProjectile.transform.LookAt(sm.mRigidbody.worldCenterOfMass);
                }
                else
                {
                    mLightningProjectile.transform.LookAt(nearest.transform.position);
                }

                //apply damage to nearest opponent
                if (isServer && ((mTimeCount += Time.deltaTime) >= mCountTilDamage))
                {
                    mTimeCount = 0;
                    nearest.TakeDamage(mDamage, this.GetType());
                }
            }
            else
            {
                if (!mStaticPotential.isPlaying)
                {
                    ActivateStaticPotential();
                }
            }
        }

        //lose static energy
        if (isServer)
        {
            mEnergyLevel -= Time.deltaTime;
            if (mEnergyLevel <= 0.0f)
            {
                Disappear();
            }
        }
    }

    public override void EndSpell()
    {
        Disappear();
    }

    private void Disappear()
    {
        mStaticPotential.gameObject.SetActive(false);
        mStaticPotential.Stop();

        mLightningProjectile.gameObject.SetActive(false);
        mLightningProjectile.Stop();

        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);
    }

    private void ActivateLightningProjectile()
    {
        mAudioSource.PlayOneShot(AttackSound);
        mStaticPotential.gameObject.SetActive(false);
        mLightningProjectile.gameObject.SetActive(true);
        mStaticPotential.Stop();
        mLightningProjectile.Play();
    }

    private void ActivateStaticPotential()
    {
        mLightningProjectile.gameObject.SetActive(false);
        mStaticPotential.gameObject.SetActive(true);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        mStaticPotential.Play();
        mLightningProjectile.Stop();
    }
}
