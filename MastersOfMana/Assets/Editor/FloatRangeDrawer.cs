using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatRange))]

public class FloatRangeDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty min, max;
        string name = "";
        //bool cache = false;

        //if (!cache)
        //{
            //get the name before it's gone
            name = property.displayName;

            //get the X and Y values
            property.Next(true);
            min = property.Copy();
            property.Next(true);
            max = property.Copy();

        //    cache = true;
        //}

        Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));

        //Check if there is enough space to put the name on the same line (to save space)
        if (position.height > 16f)
        {
            position.height = 16f;
            EditorGUI.indentLevel += 1;
            contentPosition = EditorGUI.IndentedRect(position);
            contentPosition.y += 18f;
        }

        float half = contentPosition.width / 2;
        GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);

        //show the X and Y from the point
        EditorGUIUtility.labelWidth = 28f;
        contentPosition.width *= 0.5f;
        EditorGUI.indentLevel = 0;

        // Begin/end property & change check make each field
        // behave correctly when multi-object editing.
        EditorGUI.BeginProperty(contentPosition, label, min);
        {
            EditorGUI.BeginChangeCheck();
            float newVal = EditorGUI.FloatField(contentPosition, new GUIContent("min"), min.floatValue);
            if (EditorGUI.EndChangeCheck())
                min.floatValue = newVal;
        }
        EditorGUI.EndProperty();

        contentPosition.x += half;

        EditorGUI.BeginProperty(contentPosition, label, max);
        {
            EditorGUI.BeginChangeCheck();
            float newVal = EditorGUI.FloatField(contentPosition, new GUIContent("max"), max.floatValue);
            if (EditorGUI.EndChangeCheck())
                max.floatValue = newVal;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return Screen.width < 333 ? (16f + 18f) : 16f;
    }
}
