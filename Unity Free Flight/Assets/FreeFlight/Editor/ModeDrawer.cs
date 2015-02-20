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
		List<string> mechNames = getMechanicNames ();

		SerializedProperty mechanicsNames = property.FindPropertyRelative ("mechanicNames");
		EditorGUILayout.PropertyField(mechanicsNames.FindPropertyRelative("Array.size"));
		for (int i = 0; i < mechanicsNames.arraySize; i++) {
			int selected = mechNames.IndexOf (mechanicsNames.GetArrayElementAtIndex(i).stringValue);
			selected = EditorGUILayout.Popup ("Select Mechanic:", selected, mechNames.ToArray (), GUIStyle.none);
			if (selected > -1) {
				mechanicsNames.GetArrayElementAtIndex(i).stringValue = mechNames[selected];
			}
		}
		EditorGUILayout.PropertyField (property.FindPropertyRelative ("mechanics"), true);
		EditorGUILayout.PropertyField (property.FindPropertyRelative ("defaultMechanic"), true);

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