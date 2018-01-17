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

    [Header("Visuals")]
	public GameObject explosionPrefab;
	public GameObject grenadeLeftOver;

	public Transform mesh;
    public AnimationCurve scaleX;
    public AnimationCurve scaleY;
    public AnimationCurve scaleZ;

    [Header("SFX")]
    public PitchingAudioClip[] collisionSounds;
    public PitchingAudioClip[] throwSounds;

    public AudioSource audioSource;

    private static float? sRigidMass = null;

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

        (preview.instance as PreviewSpellTrajectory).VisualizeTrajectory(caster.handTransform.position, vel, sRigidMass.Value);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();

	}

    public override void Execute(PlayerScript caster)
    {
		GrenadeBehaviour grenadeBehaviour = PoolRegistry.GetInstance(this.gameObject, 4, 4).GetComponent<GrenadeBehaviour>();

		grenadeBehaviour.caster = caster;

        grenadeBehaviour.gameObject.SetActive(true);

		Vector3 aimDirection = GetAimServer(caster);
		                                                      //aweful fix, this should prevent spawing the grenade inside the player due to lag
        grenadeBehaviour.Reset(caster.handTransform.position + aimDirection, caster.transform.rotation);

		grenadeBehaviour.mRigid.velocity = aimDirection * throwForce;

        //create an instance of this grenade on the client's machine
        NetworkServer.Spawn(grenadeBehaviour.gameObject, grenadeBehaviour.GetComponent<NetworkIdentity>().assetId);

		grenadeBehaviour.StartCoroutine(grenadeBehaviour.LightFuse(lifeTime));
    }

    private void OnEnable()
    {
        throwSounds.RandomElement().Play(audioSource);
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
        float f = 0;
		Vector3 scale = Vector3.zero;

        while(f < time)
        {
            float t = f / time;

            scale.x = scaleX.Evaluate(t);
            scale.y = scaleY.Evaluate(t);
            scale.z = scaleZ.Evaluate(t);

            mesh.localScale = scale;

            f += Time.deltaTime;
            yield return null;
        }

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
	    GameObject burningGround = PoolRegistry.GetInstance(grenadeLeftOver, position, Quaternion.identity, 2, 2, Pool.PoolingStrategy.OnMissRoundRobin, Pool.Activation.ReturnActivated);
	}

    [ClientRpc]
    void RpcPlayCollisionSFX()
    {
        collisionSounds.RandomElement().Play(audioSource);
    }

    protected override void ExecuteCollision_Host(Collision collision)
    {
        base.ExecuteCollision_Host(collision);
        RpcPlayCollisionSFX();
    }
}