using System;
using UnityEngine.Networking;
using UnityEngine;

/// <summary>
/// SpellBehaviours describe the procedure of a single step, included in a Spell
/// this can be the execution of castcoreography as the execution of an animation, or the instantiation of an actal Fireball prefab etc.
/// Each SpellComponent has a Reference next to the next step in the Execution of a Spell
/// </summary>
public abstract class A_SpellBehaviour : NetworkBehaviour
{
	public virtual void Awake()	{}

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
        Vector3 onPlane = Vector3.ProjectOnPlane((point - caster.GetCameraPosition()), caster.GetCameraLookDirection());
        float dotProduct = Vector3.Dot((point - onPlane), caster.GetCameraLookDirection());
        Debug.Log("dotproduct: " + dotProduct);
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

}
