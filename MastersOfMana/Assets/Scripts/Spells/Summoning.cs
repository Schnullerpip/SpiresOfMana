using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object to generally describe a spell, that will 'summon' something, meaning, that it will actually manifest something in the scene 
/// </summary>
[CreateAssetMenu(fileName = "New Summoning", menuName = "Spells/Summoning", order = 1)]
public class Summoning : A_Spell 
{
	#region implemented abstract members of A_Spell
	public override void Resolve (PlayerScript caster)
	{
	    foreach (var sc in SpellBehaviours)
	    {
	        sc.Execute(caster);
	    }
	}
	#endregion
}