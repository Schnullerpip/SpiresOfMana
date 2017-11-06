using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransformRayPlacement : EditorWindow 
{
	bool isActive = false;
	string stringActivate = "Activate";
	string stringDeactivate = "Deactivate";

	static Transform[] selectedTransforms;

	[MenuItem ("Tools/Transform Ray Placement")]
	static void Init () 
	{
		TransformRayPlacement window = (TransformRayPlacement) EditorWindow.GetWindow (typeof (TransformRayPlacement));
		window.Show();
	}

	void OnSelectionChange()
	{
		Repaint();
	}

	void OnGUI () 
	{
		selectedTransforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
		GUILayout.Label (selectedTransforms.Length+" Objects selected");

		GUI.enabled = selectedTransforms.Length != 0 || isActive;

		if(GUILayout.Button(isActive ? stringDeactivate : stringActivate))
		{
			isActive = !isActive;

			if(isActive)
			{
				SceneView.onSceneGUIDelegate += SceneGUI;
			}
			else
			{
				SceneView.onSceneGUIDelegate -= SceneGUI;
			}
		}
	}
		
	static void SceneGUI(SceneView sceneView)
	{	
		Event e = Event.current;

		int controlId = GUIUtility.GetControlID(FocusType.Passive);

		if(e.type == EventType.MouseDown && e.button == 0 && e.modifiers == EventModifiers.Shift)
		{
			GUIUtility.hotControl = controlId;
			e.Use();  //Eat the event so it doesn't propagate through the editor.

			Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				Undo.RecordObjects (selectedTransforms, "Move Objects");

				foreach (Transform t in selectedTransforms) {
					t.position = hit.point;
				}
			}
		}
	}
}
