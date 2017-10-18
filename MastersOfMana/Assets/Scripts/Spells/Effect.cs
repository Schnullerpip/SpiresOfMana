using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object to generally discribe a spell, that rather affects the player's character, but to 'instantiate' something in the scene
/// </summary>
[CreateAssetMenu(fileName = "New Effect", menuName = "Spells/Effects", order = 1)]
public class Effect : A_Spell 
{
	#region implemented abstract members of A_Spell

	public override void Resolve (PlayerScript caster)
	{
        foreach (var sb in SpellBehaviours) {
            sb.Execute(caster);
        }
	}

	#endregion
}
