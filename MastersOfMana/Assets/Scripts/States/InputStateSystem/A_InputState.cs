using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// abstract State, that defines default behaviour for substates and provides the base constructor
/// </summary>
public abstract class A_InputState : A_State{

    public A_InputState(PlayerScript player) : base(player) { }

    /*behaviour distinction
     * those are keptempty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */
    public virtual void Hurt() { }
	public virtual void Move(Vector2 input) { }
    public virtual void Aim(Vector2 input) 
	{
        //TODO: add true 360 aiming
		mPlayer.transform.Rotate(0,input.x,0);
	}
    public virtual void Jump() 
	{
		mPlayer.Jump();
	}
    public virtual void Cast_Spell_1() { }
    public virtual void Cast_Spell_2() { }
    public virtual void Cast_Spell_3() { }
}
