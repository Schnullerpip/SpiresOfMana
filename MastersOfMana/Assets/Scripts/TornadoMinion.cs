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
		NavMesh.SamplePosition(sourcePos, out navHit, maxDistance, NavMesh.AllAreas);
		navPosition = navHit.position;

        Debug.DrawRay(sourcePos + new Vector3(-.2f,0,-.2f), new Vector3(.4f, 0, .4f), Color.red, 10);
        Debug.DrawRay(sourcePos + new Vector3(-.2f, 0, .2f), new Vector3(.4f, 0, -.4f), Color.red, 10);
        Debug.DrawLine(sourcePos, navPosition, Color.red, 10);

		return navHit.hit;
	}

    public void AdjustPosition(Vector3 offset)
    {
        mAgent.Move(offset);
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

    private void Awake()
    {
        mInitalScale = transform.localScale;
        mAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnStartClient()
	{
		Appear();


        if(!isServer)
        {
            return;
        }

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

			Vector3 appliedForce = direction * force;

			if (rigid.CompareTag("Player"))
			{
				PlayerScript ps = rigid.GetComponent<PlayerScript>();
				ps.movement.RpcAddForce(appliedForce, ForceMode.VelocityChange);
				ps.healthScript.TakeDamage(damage, typeof(TornadopocalypeBehaviour));

				mHot = false;
			    if (gameObject.activeSelf && isServer)
			    {
                    RpcDisappear();
			    }
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

		if(mTarget && IsOnNavMesh())
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
