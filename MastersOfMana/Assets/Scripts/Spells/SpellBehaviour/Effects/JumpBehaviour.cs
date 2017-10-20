using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehaviour : A_EffectBehaviour 
{
	public float jumpForce = 15.0f;
	public float pullInRadius = 4.0f;
	public float pullInForce = 600;

	public override void Execute(PlayerScript caster)
	{
        caster.RpcAddForce(Vector3.up * jumpForce, (int)ForceMode.VelocityChange);
		Collider[] cols = Physics.OverlapSphere(caster.transform.position, pullInRadius);

		foreach(Collider c in cols)
		{
			if(c.attachedRigidbody != null && c.attachedRigidbody.gameObject != caster.gameObject)
			{
				if (c.attachedRigidbody.CompareTag("Player"))
				{
					PlayerScript opponent = c.attachedRigidbody.GetComponent<PlayerScript>();
					opponent.RpcAddExplosionForce(-pullInForce, caster.transform.position, pullInRadius);
				}
				else
				{
					c.attachedRigidbody.AddExplosionForce(-pullInForce, caster.transform.position, pullInRadius);
				}
			}
		}
	}
}
