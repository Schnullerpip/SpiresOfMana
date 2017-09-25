using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_Spell : ScriptableObject 
{
	public UnityEngine.UI.Image mIcon;
	public GameObject[] mSpawningObjects;

	public abstract void Cast();
}

