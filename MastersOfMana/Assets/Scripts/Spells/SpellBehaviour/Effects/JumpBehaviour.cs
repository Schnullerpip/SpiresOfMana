using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JumpBehaviour : A_EffectBehaviour 
{
	public float jumpForce = 15.0f;
	public float implosionDelay = 0.1f;
	public float pullInRadius = 4.0f;
	public float pullInForce = 8.0f;

	public GameObject vacuumPrefab;

	public override void Execute(PlayerScript caster)
	{
        //Get a jumpinstance out of the pool
        JumpBehaviour jumpBehaviour = PoolRegistry.JumpPool.Get().GetComponent<JumpBehaviour>();

        //now activate it
        jumpBehaviour.gameObject.SetActive(true);

        //create an instance of this jump on the client's machine
        NetworkServer.Spawn(jumpBehaviour.gameObject, PoolRegistry.JumpPool.assetID);

        Vector3 velocity = caster.movement.mRigidbody.velocity;
		velocity.y = jumpForce;
        caster.movement.RpcSetVelocity(velocity);

		caster.StartCoroutine(DelayedImpulse(implosionDelay, caster.transform.position, caster.transform));
        jumpBehaviour.RpcImplosion(caster.transform.position, caster.transform.rotation);
		Instantiate(vacuumPrefab, caster.transform.position, caster.transform.rotation);
	}

	[ClientRpc]
	void RpcImplosion(Vector3 position, Quaternion rotation)
	{
		Instantiate(vacuumPrefab, position, rotation);
	}

	public IEnumerator DelayedImpulse(float delay, Vector3 position, Transform casterTransform)
	{
		yield return new WaitForSeconds(delay);

		Collider[] cols = Physics.OverlapSphere(position, pullInRadius);

		List<Rigidbody> cachedRigid = new List<Rigidbody>(cols.Length);

		foreach(Collider c in cols)
		{
			if(c.attachedRigidbody != null && c.attachedRigidbody.transform != casterTransform && !cachedRigid.Contains(c.attachedRigidbody))
			{
				cachedRigid.Add(c.attachedRigidbody);

				Vector3 direction = position - c.attachedRigidbody.transform.position + Vector3.up * 0.2f;
				direction.Normalize();
				direction *= pullInForce;
				Debug.DrawRay(c.attachedRigidbody.transform.position,direction, Color.yellow, 10);

				if (c.attachedRigidbody.CompareTag("Player"))
				{
					PlayerScript opponent = c.attachedRigidbody.GetComponent<PlayerScript>();
					opponent.movement.RpcAddForce(direction, ForceMode.VelocityChange);
				}
				else
				{
					c.attachedRigidbody.AddForce(direction, ForceMode.VelocityChange);
				}
			}
		}
	}

	void OnValidate()
	{
	    if (vacuumPrefab)
	    {
            vacuumPrefab.transform.localScale = Vector3.one * pullInRadius * 2;
	    }
	}
}
