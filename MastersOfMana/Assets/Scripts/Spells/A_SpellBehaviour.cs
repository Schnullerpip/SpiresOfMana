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
	public abstract void Execute(PlayerScript caster);

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

		player.SetColliderIgnoreRaycast(true);
		bool hitSomething = Physics.Raycast(ray, out hit);
		player.SetColliderIgnoreRaycast(false);

		return hitSomething ? Vector3.Normalize(hit.point - player.handTransform.position) : player.GetCameraLookDirection();
	}
}
