using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class, that sets up the minimal needs for a state -> to know a mPlayer
/// </summary>
public class A_State
{
    private PlayerScript mPlayer;

<<<<<<< HEAD
    public A_State(PlayerScript mPlayer)
    {
        this.mPlayer = mPlayer;
    }
=======
    /*Each Player will hold his or her specific state instances in a dictionary
     * if a state change occures the 'new' state can be adressed through the player's
     * dictionary with the respective StateID as the key */
    public enum StateID { Normal, Moving, Hurt};

    //Constructor - this abstract class defines the constructor for all the subclasses
    public A_State(PlayerScript player) {
        this.player = player;
    }
    private A_State() { }

    /*behaviour distinction
     * those are keptempty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */
    public virtual void Hurt() { }
    public virtual void Move(Vector2 input) { }
    public virtual void Jump() { }
    public virtual void Cast_Spell_1() { }
    public virtual void Cast_Spell_2() { }
    public virtual void Cast_Spell_3() { }
    public virtual void Collide() { }
>>>>>>> 6b64c8611b7c834e005d2d34557aa1c407d21a78

    //prevent standard construction
    private A_State() {}

	// Use this for continuous statechecks
	public virtual void Update () {}
}
