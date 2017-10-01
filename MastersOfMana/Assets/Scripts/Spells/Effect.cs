using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Spells/Effects", order = 1)]
public class Effect : A_Spell 
{
	#region implemented abstract members of A_Spell

	public override void Cast (PlayerScript caster)
	{
		throw new System.NotImplementedException ();
	}

	#endregion
}
