using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsExtended : MonoBehaviour {

	public static void SetColor(string name, Color col)
    {
        PlayerPrefs.SetFloat(name + "r", col.r);
        PlayerPrefs.SetFloat(name + "g", col.g);
        PlayerPrefs.SetFloat(name + "b", col.b);
        PlayerPrefs.SetFloat(name + "a", col.a);
    }

    public static Color GetColor(string name, Color defaultCol)
    {
        float r = PlayerPrefs.GetFloat(name + "r", defaultCol.r);
        float g = PlayerPrefs.GetFloat(name + "g", defaultCol.g);
        float b = PlayerPrefs.GetFloat(name + "b", defaultCol.b);
        float a = PlayerPrefs.GetFloat(name + "a", defaultCol.a);
        return new Color(r, g, b, a);
    }
}
