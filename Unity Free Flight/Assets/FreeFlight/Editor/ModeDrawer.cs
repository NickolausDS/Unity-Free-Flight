using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityFreeFlight;

//HACK -- Draw for flight mode only. 
[CustomPropertyDrawer (typeof (FlightMode))]
//[CustomPropertyDrawer (typeof (GroundMode))]
//[CustomPropertyDrawer (typeof(BaseMode))]
class ModeDrawer : PropertyDrawer {
	
	// Draw the property inside the given rect
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		SerializedProperty mechanics = property.FindPropertyRelative ("mechanics");


//		EditorGUILayout.HelpBox ("Properties", MessageType.None);
		EditorGUILayout.LabelField ("Mechanics Precedence");
		EditorGUI.indentLevel++;
		for (int i = 0; i < mechanics.arraySize; i++) {
			EditorGUILayout.LabelField (mechanics.GetArrayElementAtIndex(i).FindPropertyRelative ("name").stringValue);
		}
		EditorGUI.indentLevel--;

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Default Mechanic");
		EditorGUILayout.LabelField (property.FindPropertyRelative ("defaultMechanic").FindPropertyRelative("name").stringValue);
		EditorGUILayout.EndHorizontal ();


		EditorGUI.indentLevel++;
		//Draw all the normal anonymous mechanics.
		for (int i = 0; i < mechanics.arraySize; i++) {
			EditorGUILayout.PropertyField (mechanics.GetArrayElementAtIndex(i), true);
		}

		//Draw the special derived mechanics.
		List<string> mechNames = getMechanicNames ();
		foreach (string name in mechNames) {
			EditorGUILayout.PropertyField (property.FindPropertyRelative (name.ToLower()), true);
		}
		EditorGUI.indentLevel--;
	}

	public List<string> getMechanicNames() {
		List<Type> mechanicTypes = UnityTypeResolver.GetAllSubTypes (typeof(Mechanic));
		List<string> names = new List<string> ();
		foreach (Type t in mechanicTypes) {
			names.Add (t.Name);
		}
		
		return names;
	}
	


}