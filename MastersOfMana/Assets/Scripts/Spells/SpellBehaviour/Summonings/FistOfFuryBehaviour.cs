using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class FistOfFuryBehaviour : A_SummoningBehaviour
{
    [SerializeField] private float mExplosionForce;
    [SerializeField] private float mPushDownForce;
    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private ExplosionFalloff mExplosionFalloff;
    [SerializeField] private float mExplosionRadius;
    [SerializeField] private float mMaxDistance;

	public PreviewSpell previewPrefab;

    //will store the transform.position of the caster when he casted - the difference between that and he collisionpoint will be a factor to the resulting damage
    private Vector3 castPosition;

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);
		RaycastHit hit;

		if(Physics.Raycast(caster.transform.position + Vector3.up * 0.2f, Vector3.down, out hit))
		{
			previewPrefab.instance.Move(hit.point);
		}
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
		previewPrefab.instance.Deactivate();
	}

    public override void Execute(PlayerScript caster)
    {
        //get a fistoffury object
        //FistOfFuryBehaviour fof = PoolRegistry.FistOfFuryPool.Get(Pool.Activation.ReturnActivated).GetComponent<FistOfFuryBehaviour>();
        FistOfFuryBehaviour fof = PoolRegistry.GetInstance(this.gameObject, 4, 4).GetComponent<FistOfFuryBehaviour>();
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
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        transform.parent = caster.transform;
    }

    protected override void ExecuteTriggerEnter_Host(Collider collider)
    {
        if (collider.isTrigger) return;

        GetComponent<Collider>().enabled = false;

        //spawn an explosion
        RpcExplosion(caster.transform.position);

        //unparent it
        transform.parent = null;

        Vector3 distanceVector = caster.transform.position - castPosition;
        float distance = Mathf.Clamp(Vector3.Magnitude(distanceVector), 3.0f, mMaxDistance); //so there will ALWAYS be a little damage at least
        float resultingHeightFactor = distance/mMaxDistance;
        resultingHeightFactor = Mathf.Clamp(resultingHeightFactor, 0.0f, 1.0f); // if the maxDistance was topped, dont let the damage escalate

        Debug.Log("resultingHeightFactor: " + resultingHeightFactor);

        caster.SetColliderIgnoreRaycast(true);

        //apply Explosion force and damage
		ExplosionDamage(caster.transform.position + Vector3.up * 0.8f/*so the terrain is not hit*/, mExplosionRadius, mExplosionFalloff, new List<HealthScript>(), resultingHeightFactor);

        caster.SetColliderIgnoreRaycast(false);

        //remove the fistoffury object on all clients
        StartCoroutine(DestroyNextFrame());

        //Set state of player to normal
        caster.RpcSetEffectState(EffectStateSystem.EffectStateID.Normal);
    }

    public IEnumerator DestroyNextFrame()
    {
        yield return 0;//wait 1 frame

        NetworkServer.UnSpawn(gameObject);
        gameObject.SetActive(false);
    }

    [ClientRpc]
    void RpcExplosion(Vector3 position)
    {
        GameObject explosion = /*PoolRegistry.*/Instantiate(explosionPrefab);
        explosion.transform.position = position;
        //explosion.transform.localScale = new Vector3(explosionAmplitude, explosionAmplitude, explosionAmplitude);
        explosion.SetActive(true);
        //NetworkServer.Spawn(explosion);
    }

    protected override void ExecuteCollision_Host(Collision collision) { }
}