using System;
using UnityEngine.Networking;

/// <summary>
/// SpellBehaviours describe the procedure of a single step, included in a Spell
/// this can be the execution of castcoreography as the execution of an animation, or the instantiation of an actal Fireball prefab etc.
/// Each SpellComponent has a Reference next to the next step in the Execution of a Spell
/// </summary>
public abstract class A_SpellBehaviour : NetworkBehaviour
{
    //relevant data like damage, manacost etc.
    public A_Spell referencedSpell;

    public virtual void Awake()
    {
        //make sure referencedSpell was attached in Editor
        if (!referencedSpell)
        {
            throw new MissingMemberException();
        }
    }

    public abstract void Execute(PlayerScript caster);
}
