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

    public static void SetResolution(string name, Resolution resolution)
    {
        PlayerPrefs.SetInt(name + "width", resolution.width);
        PlayerPrefs.SetInt(name + "height", resolution.height);
    }

    public static Resolution GetResolution(string name, Resolution defaultResolution)
    {
        Resolution res = new Resolution();
        res.width = PlayerPrefs.GetInt(name + "width", defaultResolution.width);
        res.height = PlayerPrefs.GetInt(name + "height", defaultResolution.height);
        return res;
    }

    public static void SetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
    }

    public static bool GetBool(string name, bool defaultValue)
    {
        if (PlayerPrefs.HasKey(name))
        {
            return PlayerPrefs.GetInt(name) == 1;
        }
        return defaultValue;
    }
}
