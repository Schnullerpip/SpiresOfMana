using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class FistOfFuryBehaviour : A_SummoningBehaviour
{
    [Header("Effect on caster")]
    [SerializeField] private float mPushDownForce;
    [Header("Damage/Force-Relevant")]
    //will store the transform.position of the caster when he casted - the difference between that and he collisionpoint will be a factor to the resulting damage
    [SyncVar]
    private Vector3 castPosition;
    [SerializeField] private float mExplosionForce;
    [SerializeField] private ExplosionFalloff mExplosionFalloff;
    [SerializeField] private float mExplosionRadius;
    [SerializeField] private float mMaxDistance;

    [Header("Visuals")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private GameObject brutalityPrefab;
    [SerializeField] private GameObject flamesPrefab;
    [SerializeField] private GameObject spawnExplosionIfAirborn;
    [SerializeField] private float EffectQuantum1;
    [SerializeField] private float EffectQuantum2; 
    [SerializeField] private float EffectQuantum3; 


	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);
		RaycastHit hit;

		if(Physics.Raycast(caster.transform.position + Vector3.up * 0.2f, Vector3.down, out hit))
		{
            preview.instance.Move(hit.point);
		}
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();
	}

    public override void Execute(PlayerScript caster)
    {
        //get a fistoffury object
        //FistOfFuryBehaviour fof = PoolRegistry.FistOfFuryPool.Get(Pool.Activation.ReturnActivated).GetComponent<FistOfFuryBehaviour>();
        FistOfFuryBehaviour fof = PoolRegistry.GetInstance(this.gameObject, 1, 1).GetComponent<FistOfFuryBehaviour>();
        fof.caster = caster;
        fof.casterObject = caster.gameObject;
        fof.transform.position = fof.castPosition = caster.transform.position;
        fof.transform.parent = caster.transform;
        fof.GetComponent<Collider>().enabled = true;
        fof.gameObject.SetActive(true);

        //spawn it on all clients
        NetworkServer.Spawn(fof.gameObject);

        //check whether caster is airborn or grounded
        if (!caster.movement.feet.IsGrounded())
        {
            //set caster's state so he or she doesnt get falldamage
            caster.SetEffectState(EffectStateSystem.EffectStateID.NoFallDamage);
            caster.movement.RpcAddForce(Vector3.down * mPushDownForce, ForceMode.VelocityChange);

            //activate initial spawneffect
            GameObject spawnExplosion = PoolRegistry.GetInstance(spawnExplosionIfAirborn, caster.transform.position, Quaternion.identity, 2, 2, Pool.PoolingStrategy.OnMissSubjoinElements, Pool.Activation.ReturnActivated);
            NetworkServer.Spawn(spawnExplosion);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        transform.parent = caster.transform;
    }

    private new void OnTriggerEnter(Collider collider)
    {
        if (collider.isTrigger) return;

        Vector3 distanceVector = caster.transform.position - castPosition;
        float distance = Mathf.Clamp(Vector3.Magnitude(distanceVector), 3.0f, mMaxDistance); //so there will ALWAYS be a little damage at least
        float resultingHeightFactor = distance/mMaxDistance;
        resultingHeightFactor = Mathf.Clamp(resultingHeightFactor, 0.0f, 1.0f); // if the maxDistance was topped, dont let the damage escalate

        //spawn an explosion
        Explosion(caster.transform.position, resultingHeightFactor);

        if (isServer)
        {
            GetComponent<Collider>().enabled = false;

            //unparent it
            transform.parent = null;
            caster.SetColliderIgnoreRaycast(true);

            //apply Explosion force and damage
            ExplosionDamage(caster.transform.position + Vector3.up * 0.8f/*so the terrain is not hit*/, mExplosionRadius, mExplosionFalloff, new List<HealthScript>(), resultingHeightFactor, 1 + resultingHeightFactor);

            caster.SetColliderIgnoreRaycast(false);

            //remove the fistoffury object on all clients
            StartCoroutine(DestroyNextFrame());

            //Set state of player to normal
            caster.RpcSetEffectState(EffectStateSystem.EffectStateID.Normal);
        }
    }

    public IEnumerator DestroyNextFrame()
    {
        yield return null;//wait 1 frame

        NetworkServer.UnSpawn(gameObject);
        gameObject.SetActive(false);
    }

    [ClientRpc]
    void RpcExplosion(Vector3 position, float height)
    {
        Explosion(position, height);
    }

    private void Explosion(Vector3 position, float height)
    {
        {
            GameObject explosion = PoolRegistry.GetInstance(explosionPrefab, 2, 2);
            explosion.transform.position = position;
            explosion.SetActive(true);
        }

        if (height > EffectQuantum1)
        {
            GameObject decal = PoolRegistry.GetInstance(decalPrefab, 1, 1);
            decal.transform.position = position;
            decal.SetActive(true);
        }
        if (height > EffectQuantum2)
        {
            GameObject brutality = PoolRegistry.GetInstance(brutalityPrefab, 1, 1);
            brutality.transform.position = position;
            brutality.SetActive(true);
        }
        if (height > EffectQuantum3)
        {
            GameObject flames = PoolRegistry.GetInstance(flamesPrefab, 1, 1);
            flames.transform.position = position;
            flames.SetActive(true);
        }
    }

    protected override void ExecuteCollision_Host(Collision collision) { }
}