using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Summoning", menuName = "Spells/Summoning", order = 1)]
public class Summoning : A_Spell 
{
	#region implemented abstract members of A_Spell
	public override void Cast (PlayerScript caster)
	{
	    foreach (var sc in SpellBehaviours)
	    {
	        sc.Execute(caster);
	    }
	}
	#endregion
}