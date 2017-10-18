using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class to describe spells, scriptable object classes can extend this, to further divide into classes with different traits
/// </summary>
public abstract class A_Spell : ScriptableObject 
{
    //Spelldata
	public Sprite icon;
    public float coolDownInSeconds;
    public float castDurationInSeconds;
    public float resolveDurationInSeconds;
    public int spellID;

    //holds prefabs, representing actual manifests of spells like a fireball - technically a dash spell will have a meshless prefab only holding the behaviour for example
    public A_SpellBehaviour[] SpellBehaviours;


    /// <summary>
    /// unified interface for calling a Spell
    /// </summary>
    /// <param name="caster"></param>
    public abstract void Resolve(PlayerScript caster);
}