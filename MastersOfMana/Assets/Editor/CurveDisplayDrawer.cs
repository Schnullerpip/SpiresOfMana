using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CurveDisplayAttribute))]
public class CurveDisplayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true) * 5;
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {

        EditorGUI.CurveField(new Rect(position.x,position.y,position.width,position.height), "Emission Curve (readonly)", property.animationCurveValue, Color.red, new Rect());
    }
}