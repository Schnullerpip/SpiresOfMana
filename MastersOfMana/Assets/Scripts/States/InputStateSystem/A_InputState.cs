using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// abstract State, that defines default behaviour for substates and provides the base constructor
/// </summary>
public abstract class A_InputState : A_State{

	protected Rewired.Player playerInput;

    public A_InputState(PlayerScript player) : base(player) 
	{ 
		playerInput = player.GetRewired();
	}

    /*behaviour distinction
     * those are kept empty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */

	protected bool mPreviewActive;
	protected bool mFocus;

    public override void UpdateLocal()
    {
        //any state should consider an abrupt "pause" button event and react accordingly
        if (playerInput.GetButtonDown("IngameMenu"))
        {
            player.GetPlayerSpells().StopPreview();
            SetPreview(false);
        }
    }
    

	public void TogglePreview()
	{
		mPreviewActive = !mPreviewActive;
		player.GetPlayerAnimation().HoldingSpell(mPreviewActive);
	}

	public void SetPreview(bool value)
	{
		mPreviewActive = value;
		player.GetPlayerAnimation().HoldingSpell(mPreviewActive);
	}

	public virtual void Move(Vector2 input) { }

	public virtual void Aim(Vector2 aimInput) 
	{
//		player.aim.ValidateFocusTargetView();

		if(player.aim.HasFocusTarget())
		{
			//TODO: get actual rewired controller type
//			player.aim.RefineAim(aimInput, Rewired.ControllerType.Mouse);
			player.aim.LookAtFocusTarget();
		}
		else
		{
//			player.aim.ResetRefinement();
			player.aim.Aim(aimInput);
		}
	}

    public virtual void ChooseSpell(int idx)
    {
		PlayerSpells playerSpells = player.GetPlayerSpells();
		playerSpells.StopPreview();
        playerSpells.SetCurrentSpellslotID(idx);
        playerSpells.CmdChooseSpellslot(idx);
    }

    //casting the chosen spell
    public virtual void CastSpell()
    {
		mPreviewActive = false;
		player.GetPlayerSpells().StopPreview();
        player.castStateSystem.current.CastCmdSpell();
		if(player.GetPlayerSpells().GetCurrentspell().cooldown > 0)
		{
			player.GetPlayerAnimation().HoldingSpell(false);
			player.GetPlayerSpells().PlayFailSFX();
		}
    }

	protected Vector3 World2DToLocal3D (Vector2 world2D, Transform transform)
	{
		//from global to local space
		return transform.TransformDirection (world2D.x, 0, world2D.y);
	}
}
