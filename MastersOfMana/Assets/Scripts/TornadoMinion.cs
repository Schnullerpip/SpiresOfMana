using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class TornadoMinion : NetworkBehaviour {

	public AnimationCurve growCurve;
	public AnimationCurve dieCurve;

	private PlayerScript mTarget;
	private NavMeshAgent mAgent;

	public float lifeTime = 4;
	public float force = 10;
	public int damage = 12;

	private float mCurrentLifetime;

	private bool mHot = true;
	private Vector3 mInitalScale;

	public static bool GetPositionOnMesh(Vector3 sourcePos, float maxDistance, out Vector3 navPosition)
	{
		NavMeshHit navHit;
		bool returnValue = NavMesh.SamplePosition(sourcePos, out navHit, maxDistance, NavMesh.AllAreas);
		navPosition = navHit.position;
		return returnValue;
	}

	public bool IsOnNavMesh()
	{
		return mAgent.isOnNavMesh;
	}

	public void Appear()
	{
		StartCoroutine(Grow());
	}

	[ClientRpc]
	public void RpcDisappear()
	{
		//the action that is invoked at the end of the disappear animation
		System.Action doneAction = null;

		//if its on the client, dont do anything. 
		if(isServer)
		{
			//on the server actually unspawn the minion
			doneAction = Unspawn;
		}

		StartCoroutine(Die(doneAction));

	}

	/// <summary>
	/// Unspawns this instance on client and server.
	/// </summary>
	void Unspawn()
	{
		NetworkServer.UnSpawn(this.gameObject);
		gameObject.SetActive(false);
	}

	void OnEnable()
	{
		mHot = true;
		mCurrentLifetime = lifeTime;
	}

	public override void OnStartClient()
	{
		mInitalScale = transform.localScale;
		Appear();

		if(!isServer)
		{
			return;
		}

		mAgent = GetComponent<NavMeshAgent>();

		OnTriggerEvent trigger = GetComponentInChildren<OnTriggerEvent>();
		trigger.onTriggerEnter += TriggerEnter;
	}
		
	void TriggerEnter(Collider other)
	{
		if(!mHot)
			return;
		
		Rigidbody rigid = other.attachedRigidbody;

		if(rigid)
		{
			Vector3 pushPos = transform.position;
			pushPos.y = rigid.worldCenterOfMass.y;

			Vector3 direction = rigid.worldCenterOfMass - pushPos;
			direction.Normalize();

//			direction += (Quaternion.LookRotation(direction) * (clockWise ? Vector3.right : Vector3.left)) * tangentForce;

			Vector3 appliedForce = direction * force;

			if (rigid.CompareTag("Player"))
			{
				PlayerScript ps = rigid.GetComponent<PlayerScript>();
				ps.movement.RpcAddForce(appliedForce, ForceMode.VelocityChange);
				ps.healthScript.TakeDamage(damage, typeof(TornadopocalypeBehaviour));

				mHot = false;
				RpcDisappear();
			}
			else
			{
				rigid.AddForce(appliedForce, ForceMode.VelocityChange);
			}

		}
	}

	void Update()
	{
		if(!isServer)
		{
			return;
		}

		if(mTarget)
		{
			mAgent.SetDestination(mTarget.transform.position);
		}

		mCurrentLifetime -= Time.deltaTime;

		if(mCurrentLifetime <= 0 && mHot)
		{
			mHot = false;
			RpcDisappear();
		}
	}

	public void SetTarget(PlayerScript player)
	{
		mTarget = player;
	}

	IEnumerator Grow () 
	{
		float t = 0;
		while(t < 1)
		{

			transform.localScale = mInitalScale * growCurve.Evaluate(t);
			t += Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator Die (System.Action onDone = null) 
	{
		float t = 0;
		while(t < 1)
		{

			transform.localScale = mInitalScale * dieCurve.Evaluate(t);
			t += Time.deltaTime;

			yield return null;
		}

		if(onDone != null)
			onDone.Invoke();
	}
}
