using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LightningAuraBehaviour : A_SummoningBehaviour
{
    [SerializeField] private float mInitialEnergyLevel = 50;
    [SerializeField] private float mCountTillDamage = 1.0f;
    [SerializeField] private int mDamage = 1;
    private float mEnergyLevel;
    [SerializeField] private float mEnergyDecreaseFactor = 0.1f;

    [SerializeField] private A_Spell mMySpell;

    [SerializeField]
    private ParticleSystem mStaticPotential;
    [SerializeField]
    private ParticleSystem mLightningProjectile;

    //the list that captures which players are shot
    private List<PlayerScript> mAlreadyCaught;
    //so the lightning aura wont hurt the caster
    private PlayerScript caster;
    [SyncVar]
    private GameObject casterObject;

    public override void OnStartClient()
    {
        if (!isServer)
        {
            caster = casterObject.GetComponent<PlayerScript>();
        }
    }

    void Start()
    {
        mAlreadyCaught = new List<PlayerScript>();
        mEnergyLevel = mInitialEnergyLevel;
    }

    public override void Execute(PlayerScript caster)
    {
        LightningAuraBehaviour la = PoolRegistry.Instantiate(this.gameObject).GetComponent<LightningAuraBehaviour>();
        la.transform.position = caster.movement.mRigidbody.worldCenterOfMass;
        la.gameObject.SetActive(true);
        la.transform.rotation = Quaternion.Euler(Vector3.zero);
        la.caster = caster;
        la.casterObject = caster.gameObject;
        la.mEnergyLevel = mInitialEnergyLevel;//should work in Start also
        NetworkServer.Spawn(la.gameObject);
    }

    public new void OnTriggerEnter(Collider other)
    {
        PlayerScript ps = other.GetComponentInParent<PlayerScript>();
        if (ps && ps != caster && !mAlreadyCaught.Contains(ps))
        {
            mAlreadyCaught.Add(ps);
           if (!mStaticPotential.isPlaying)
            {
                ActivateLightningProjectile();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        PlayerScript ps = other.GetComponentInParent<PlayerScript>();

        if (ps)
        {
            mAlreadyCaught.Remove(ps);
            if (mAlreadyCaught.Count == 0)
            {
                ActivateStaticPotential();
            }
        }
    }

    void FixedUpdate()
    {
        if (caster)
        {
            //reposition
            transform.position = caster.movement.mRigidbody.worldCenterOfMass;
        }
    }

    private float mTimeCount = 0;
    void Update()
    {
        //as long as we dont have a caster, do nothing
        if (!caster)
        {
            return;
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
            PlayerScript nearest = mAlreadyCaught[0];
            float distance = Vector3.Distance(caster.transform.position, nearest.transform.position);
            for (int i = 1; i < mAlreadyCaught.Count; ++i)
            {
                float dist = Vector3.Distance(mAlreadyCaught[i].transform.position, caster.transform.position);
                if (dist < distance)
                {
                    nearest = mAlreadyCaught[i];
                }
            }
            //since mAlreadyCaught is not empty we assume, that mLightningProjectile is already active
            //shoot the lightning projectile at nearest opponent
            var vel = mLightningProjectile.velocityOverLifetime;
            Vector3 lightningDirection =
                Vector3.Normalize(nearest.movement.mRigidbody.worldCenterOfMass - transform.position);
            lightningDirection *= 30;
            vel.x = lightningDirection.x;
            vel.y = lightningDirection.y;
            vel.z = lightningDirection.z;

            //apply damage to nearest opponent
            mTimeCount += Time.deltaTime;
            if (mTimeCount >= mCountTillDamage)
            {
                nearest.healthScript.TakeDamage(mDamage);
            }
        }

        //lose static energy
        if (isServer)
        {
            mEnergyLevel -= mEnergyLevel*mEnergyDecreaseFactor * Time.deltaTime;
            if (mEnergyLevel < 1.0f)
            {
                Disappear();
            }
        }
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
