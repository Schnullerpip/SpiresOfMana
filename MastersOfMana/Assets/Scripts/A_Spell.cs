using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_Spell : ScriptableObject 
{
	public UnityEngine.UI.Image icon;
	public GameObject[] spawningObjects;

	public abstract void Cast();
}

