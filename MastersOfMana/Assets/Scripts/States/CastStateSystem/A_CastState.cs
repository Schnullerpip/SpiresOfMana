using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_CastState : A_State{
    public A_CastState(PlayerScript player) : base(player) { }

    /*
     * handy helper methods 
     */
    /// <summary>
    /// Calculates the aim direction for a player considering its camerarig, that is only o the local player! The resulting Vector3 can 
    /// be passed to the spell Commands, so the server can update its aiming direction, the moment it is supposed to cast a spell
    /// </summary>
    protected Vector3 CalculateAimDirection()
    {
        RaycastHit hit;
        return player.cameraRig.CenterRaycast(out hit) ? Vector3.Normalize(hit.point - player.handTransform.position) : player.lookDirection;
    }

    /*behaviour distinction
     * those are keptempty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */

    /// <summary>
    /// the method that handles reducing the spells' cooldowns so they can be cast again
    /// </summary>
    public abstract void ReduceCooldowns();

    /// <summary>
    /// will invoke the spell that is maped at spellslot 1
    /// </summary>
    public abstract void CastCmdSpellslot_1();

    /// <summary>
    /// will invoke the spell that is maped at spellslot 2
    /// </summary>
    public abstract void CastCmdSpellslot_2();

    /// <summary>
    /// will invoke the spell that is maped at spellslot 3
    /// </summary>
    public abstract void CastCmdSpellslot_3();

}
