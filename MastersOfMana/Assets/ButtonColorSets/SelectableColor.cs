using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableColor : MonoBehaviour 
{
    public ButtonColorSet colorSet;

    #if UNITY_EDITOR
    [ContextMenu("Create ColorSet")]
    private void CreateColorSet()
    {
        Selectable s = GetComponent<Selectable>();
        ButtonColorSet asset = ScriptableObject.CreateInstance<ButtonColorSet>();

        UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/"+s.name+"ColorSet.asset");

        asset.colors = s.colors;

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.EditorUtility.FocusProjectWindow();
        UnityEditor.Selection.activeObject = asset;

        colorSet = asset;
    }
    #endif

    private void OnValidate()
    {
        Selectable s = GetComponent<Selectable>();
        if(s && colorSet && s.colors != colorSet.colors)
        {
            s.colors = colorSet.colors;
        }
    }
}
