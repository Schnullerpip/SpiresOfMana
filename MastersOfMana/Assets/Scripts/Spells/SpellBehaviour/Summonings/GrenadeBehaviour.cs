using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// The specific behaviour of the fireball, that is manifested in the scene
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GrenadeBehaviour : A_ServerMoveableSummoning
{
	public float throwForce = 20;
	public float lifeTime = 3;

	public ExplosionFalloff explosionFalloff;

	public float explosionRadius = 7;
	public float explosionTime = 0.5f;

	public GameObject explosionPrefab;
	public GameObject grenadeMesh;

	private static float? sRigidMass = null;

//    private Rigidbody mStickTo = null;
//    private Vector3 mStickPosition;

    public override void Awake()
    {
        base.Awake();
        if (!mRigid)
        {
            //cant find a rigid body!!!
            throw new MissingMemberException();
        }
    }

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

		if(sRigidMass == null)
		{
			sRigidMass = GetComponent<Rigidbody>().mass;
		}

		Vector3 vel = GetAimClient(caster) * throwForce;

        preview.instance.SetAvailability(caster.CurrentSpellReady());
        (preview.instance as PreviewSpellTrajectory).VisualizeTrajectory(caster.handTransform.position, vel, sRigidMass.Value);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();

	}

    public override void Execute(PlayerScript caster)
    {
		//GrenadeBehaviour grenadeBehaviour = PoolRegistry.GrenadePool.Get().GetComponent<GrenadeBehaviour>();
		GrenadeBehaviour grenadeBehaviour = PoolRegistry.GetInstance(this.gameObject, 4, 4).GetComponent<GrenadeBehaviour>();
        //create an instance of this grenade on the client's machine
		grenadeBehaviour.caster = caster;

        grenadeBehaviour.gameObject.SetActive(true);

		Vector3 aimDirection = GetAimServer(caster);
		
		grenadeBehaviour.Reset(caster.handTransform.position, caster.transform.rotation);
		grenadeBehaviour.mRigid.velocity = aimDirection * throwForce;

		NetworkServer.Spawn(grenadeBehaviour.gameObject, grenadeBehaviour.GetComponent<NetworkIdentity>().assetId);

		grenadeBehaviour.StartCoroutine(grenadeBehaviour.LightFuse(lifeTime));
    }

	void Reset (Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;

		mRigid.Reset();
		mRigid.isKinematic = false;
	}

	IEnumerator LightFuse(float time)
	{
		//TODO cache waitforseconds
		yield return new WaitForSeconds(time);
		mRigid.isKinematic = true;

		if(isServer)
		{
			ExplosionDamage(mRigid.position, explosionRadius, explosionFalloff);

			RpcExplosion(transform.position, transform.rotation);

			//wait 1 frame
			yield return 0;

			NetworkServer.UnSpawn(gameObject);
			gameObject.SetActive(false);
		}
	}

	[ClientRpc]
	void RpcExplosion(Vector3 position, Quaternion rotation)
	{
        Instantiate(explosionPrefab,position,rotation);
	}
}