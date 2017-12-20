using System;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// SpellBehaviours describe the procedure of a single step, included in a Spell
/// this can be the execution of castcoreography as the execution of an animation, or the instantiation of an actal Fireball prefab etc.
/// Each SpellComponent has a Reference next to the next step in the Execution of a Spell
/// </summary>
public abstract class A_SpellBehaviour : NetworkBehaviour
{
    public abstract void Execute(PlayerScript caster);

	public virtual void Preview(PlayerScript caster) {}

	public virtual void StopPreview(PlayerScript caster) {}

    /// <summary>
    /// references the spell's caster, must be set in OnStartClient!!
    /// </summary>
    protected PlayerScript caster = null;
    public PlayerScript GetCaster()
    {
        return caster;
    }
    /// <summary>
    /// This is only used for initializing the caster reference on clientside!
    /// GameObjects (or rather their networkID can be passed down via syncvar and OnStartClient guarantees, to be called after the syncvars have been synchronized,
    /// so inside OnStartClient caster can be set by getting the respective Component from the casterobject
    /// </summary>
    [SyncVar]
    protected GameObject casterObject;

    public override void OnStartClient()
    {
        //initialize the caster - serverSpell's casters can and should be set in the Execute routine, so we're only interested in Clientside calls
        if (!isServer && casterObject)
        {
            caster = casterObject.GetComponent<PlayerScript>();
        }
    }

    

	/// <summary>
	/// Gets the aim direction. This direction is from the hand transform to the position that corresponds with the center of the screen.
	/// If there was no raycast hit, its the direction of the camera.
	/// </summary>
	/// <returns>The aim.</returns>
	/// <param name="player">Player.</param>
	public Vector3 GetAim(PlayerScript player)
	{
		RaycastHit hit;
		return GetAim(player, out hit);
	}

	public Vector3 GetAimLocal(PlayerScript player)
	{
		RaycastHit hit;
		return GetAimLocal(player, out hit);
	}

	/// <summary>
	/// Gets the aim direction and the hit info. This direction is from the hand transform to the position that corresponds with the center of the screen.
	/// If there was no raycast hit, its the direction of the camera.
	/// </summary>
	/// <returns>The aim.</returns>
	/// <param name="player">Player.</param>
	/// <param name="hit">Hit.</param>
	public Vector3 GetAim(PlayerScript player, out RaycastHit hit)
	{
		Ray ray = new Ray(player.GetCameraPosition(), player.GetCameraLookDirection());

		return RayCast(player, ray, out hit);
	}

	public Vector3 GetAimLocal(PlayerScript player, out RaycastHit hit)
	{
		Ray ray = player.aim.GetCameraRig().GetCenterRay();

		return RayCast(player, ray, out hit);
	}

	private Vector3 RayCast(PlayerScript player, Ray ray, out RaycastHit hit)
	{
		player.SetColliderIgnoreRaycast(true);
		bool hitSomething = Physics.Raycast(ray, out hit);
		player.SetColliderIgnoreRaycast(false);

		if(hitSomething)
		{
			return Vector3.Normalize(hit.point - player.handTransform.position);
		}
		else
		{
			return ray.direction;
		}
	}

    /// <summary>
    ///project the casters lookdirection on the plane of the other player's position
    /// </summary>
    /// <param name="point"> point to be projected </param>
    /// <param name="caster"> provides information about it's whereabouts, as well as it's camera's </param>
    /// <param name="hitRadius"> tolerance to hit or no hit </param>
    /// <returns>whether or not point should be hit or not</returns>
    protected static bool ConfirmedHit(Vector3 point, PlayerScript caster, float hitRadius, float hitRange)
    {
        RaycastHit hit;
        Vector3 onPlane = Vector3.ProjectOnPlane((point - caster.GetCameraPosition()), -caster.GetCameraLookDirection());
        float dotProduct = Vector3.Dot((point - caster.handTransform.position), caster.GetCameraLookDirection());
        return
            /*hit by raw aim?*/
            onPlane.sqrMagnitude <= hitRadius*hitRadius &&
            //hit object is infront of me?
            dotProduct > 0 &&
            /*direct sight? - avoid hit when an obstacle is inbetween*/
            Physics.Raycast(new Ray(point, (caster.movement.mRigidbody.worldCenterOfMass - point).normalized), out hit) &&
            hit.distance <= hitRange &&
            hit.transform.gameObject == caster.gameObject;
    }

    protected static void IterateCollidersAndApply<H>(Collider[] array, Action<H> action)
    {
        foreach (var element in array)
        {
            Rigidbody rigid = element.attachedRigidbody;
            H h;
            if (rigid)
            {
                h = rigid.GetComponentInParent<H>();
                if (h != null)
                {
                    action(h);
                }
            }
        }
    }

	[System.Serializable]
	public struct ExplosionFalloff
	{
		public float force;
		public int damage;
		public AnimationCurve falloff;

		public float EvaluateForce(float t)
		{
			return falloff.Evaluate(t) * force;
		}

		public int EvaluateDamage(float t)
		{
			return Mathf.RoundToInt(falloff.Evaluate(t) * damage);
		}
	}

	/// <summary>
	/// Applies an explosion with the provided parameters.
	/// </summary>
	/// <param name="explosionOrigin">Explosion origin.</param>
	/// <param name="radius">Radius.</param>
	/// <param name="explFalloff">Explosion falloff.</param>
	/// <param name="excluded">A list of Healthscript that should be skipped. eg these already got hit.</param>
	protected void ExplosionDamage(Vector3 explosionOrigin, float radius, ExplosionFalloff explFalloff, List<HealthScript> excluded = null, float externalDamageFactor = 1.0f)
	{

		//overlap a sphere at the hit position
	    int layerId = 2; //ignore raycast layer
	    int mask = 1 << layerId;
	    mask = ~mask;
		Collider[] colliders = Physics.OverlapSphere(explosionOrigin, radius, mask);

		List<float> affectFactors = new List<float>();

		//TODO: TEMP SOLUTION
		List<Rigidbody> cachedRigidbodies = new List<Rigidbody>();
		List<HealthScript> cachedHealth = new List<HealthScript>();

		float radiusSqr = radius * radius;

		foreach(Collider c in colliders)
		{
			float affect;

			//affect non static rigidbodys, but only once
			if(!c.gameObject.isStatic && c.attachedRigidbody && !cachedRigidbodies.Contains(c.attachedRigidbody))
			{
				//raycast from the center of the explosion to every collider withing the radius
				RaycastHit hit;

				//early check if the startposition is inside the collider, this usally happens when the player fires a fireball directly into a wall
				if(!c.bounds.Contains(explosionOrigin))
				{
					Ray ray = new Ray(explosionOrigin, c.attachedRigidbody.worldCenterOfMass - explosionOrigin);
					if(Physics.Raycast(ray, out hit, radius))
					{
						//if the hit collider doesnt share the same rigidbody
						if(hit.collider.attachedRigidbody != c.attachedRigidbody)
						{
							//skip it since there is something in between the 2
							Debug.Log(hit.collider.name+" was in the way of an "+GetType().ToString()+" explosion trying to hit "+ c.attachedRigidbody +". " +
								"This message is temporarily here to test if this explosionblocking works properly. Delete it when the time comes. dunno when ¯\\_(ツ)_/¯");
							continue;
						}
					}
					//if we somehow didnt hit anything
					else
					{
						continue;
					}	
				}

				cachedRigidbodies.Add(c.attachedRigidbody);

				Vector3 forceVector = c.attachedRigidbody.worldCenterOfMass - explosionOrigin; 

				affect = 1 - forceVector.sqrMagnitude / radiusSqr;
				
				forceVector.Normalize();
				forceVector *= explFalloff.EvaluateForce(affect);

				//check wheather or not we are handling a player or just some random rigidbody
				if (c.attachedRigidbody.CompareTag("Player"))
				{
					PlayerScript ps = c.attachedRigidbody.GetComponent<PlayerScript>();
					ps.movement.RpcAddForce(forceVector, ForceMode.VelocityChange);
				}
				else
				{
					c.attachedRigidbody.AddForce(forceVector, ForceMode.VelocityChange);
				}
			}
			else
			{
				affect = 1 - (c.transform.position - explosionOrigin).sqrMagnitude / radiusSqr;
			}

			HealthScript health = c.GetComponentInParent<HealthScript>();

			//collider has a healthscript
			if(health)
			{
				//if its in the exclude list
				if(excluded != null && excluded.Contains(health))
				{
					continue;
				}
					
				//only hurt each healtscript once, even if it has multiple collider
				if(!cachedHealth.Contains(health))
				{
					cachedHealth.Add(health);
					affectFactors.Add(affect);
				}
			}
		}
	
		//this is done after the overlapshere loop, that way, walls and other obstacles where able to block the explosion before dying
		for (int i = 0; i < cachedHealth.Count; i++) 
		{
			cachedHealth[i].TakeDamage(Mathf.RoundToInt(explFalloff.EvaluateDamage(affectFactors[i]) * externalDamageFactor), GetType());
		}
	}

    public virtual void Awake()
    {
        GameManager.OnRoundEnded += EndSpell;
    }

    public virtual void OnDestroy()
    {
        GameManager.OnRoundEnded -= EndSpell;
    }

    /// <summary>
    /// Stop the spell execution and clean up any remaining spell parts
    /// </summary>
    public virtual void EndSpell()
    {
        if (isServer)
        {
            NetworkServer.UnSpawn(this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
