using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// abstract State, that defines default behaviour for substates and provides the base constructor
/// </summary>
public abstract class A_InputState : A_State{

    public A_InputState(PlayerScript player) : base(player) { }

    /*behaviour distinction
     * those are kept empty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */
    public virtual void Hurt(float amount) { }
	public virtual void Move(Vector2 input) { }

    public virtual void Jump() 
	{
		player.Jump();
	}
    public virtual void Cast_Spell_1() 
	{  
		RaycastHit hit = CameraRaycast();
		if(hit.collider != null)
		{
			player.DebugRayFromHandToPosition(hit.point);
		}
		else
		{
			Debug.Log("No RaycastHit");
		}
	}

	private RaycastHit CameraRaycast()
	{
		Camera cam = player.cameraRig.GetCamera();
		Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		return hit;
	}

    public virtual void Cast_Spell_2() { }
    public virtual void Cast_Spell_3() { }

	public virtual void StartFocus()
	{
		player.StartFocus();
	}

	public virtual void StopFocus()
	{
		player.StopFocus();
	}
}
