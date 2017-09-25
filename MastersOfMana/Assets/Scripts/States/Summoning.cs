using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Summoning", menuName = "Spells/Summoning", order = 1)]
public class Summoning : A_Spell 
{
	#region implemented abstract members of A_Spell

	public override void Cast ()
	{
		throw new System.NotImplementedException ();
	}

	#endregion
}