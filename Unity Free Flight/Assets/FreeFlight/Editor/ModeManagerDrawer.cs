using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityFreeFlight;

//class ModeDrawer : PropertyDrawer {
[CustomPropertyDrawer (typeof(ModeManager))]
public class ModeManagerDrawer : PropertyDrawer {


	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
//		SerializedProperty cm = property.FindPropertyRelative ("currentMode");
//		EditorGUILayout.PropertyField (property.FindPropertyRelative("_activeMode"));


//		SerializedProperty flightMode = property.FindPropertyRelative ("flightMode");
//		EditorGUILayout.PropertyField (flightMode.FindPropertyRelative ("name"));


//		EditorGUILayout.PropertyField (property.FindPropertyRelative ("flightMode"), true);
//		EditorGUILayout.PropertyField (property.FindPropertyRelative ("groundMode"), true);
		displayMode (position, property.FindPropertyRelative ("flightMode"), label);
//		displayMode (position, property.FindPropertyRelative ("groundMode"), label);


		//This is how we *should* display the managers
//		SerializedProperty modes = property.FindPropertyRelative ("managers");
//		for (int i = 0; i < modes.arraySize; i++) {
////			if (modes.GetArrayElementAtIndex (i).serializedObject != null) {
//				EditorGUILayout.PropertyField (property.GetArrayElementAtIndex(i), true);
//
////			}
//		}


	}


	public void displayMode(Rect position, SerializedProperty mode, GUIContent label) {
		EditorGUILayout.LabelField (mode.name);
		EditorGUILayout.PropertyField (mode.FindPropertyRelative ("alwaysApplyPhysics"));


//		EditorGUILayout.PropertyField (mode.FindPropertyRelative ("mechanics"), true);

		List<string> mechanicNames = getMechanicNames ();

		SerializedProperty flightModeDefaultMechanicName = mode.FindPropertyRelative ("defaultMechanicTypeName");
		int theNewValue = EditorGUILayout.Popup(mechanicNames.IndexOf (
			flightModeDefaultMechanicName.stringValue), mechanicNames.ToArray());
		if (theNewValue > -1)
			flightModeDefaultMechanicName.stringValue = mechanicNames[theNewValue];

		SerializedProperty flightModeMechTypeNames = mode.FindPropertyRelative ("mechanicTypeNames");
		for (int i = 0; i < flightModeMechTypeNames.arraySize; i++) {
//			EditorGUILayout.PropertyField (mechanics.GetArrayElementAtIndex(i), false);
//			EditorGUILayout.LabelField ( flightModeMechTypeNames.GetArrayElementAtIndex(i).stringValue);
			int newValue = EditorGUILayout.Popup(mechanicNames.IndexOf (
				flightModeMechTypeNames.GetArrayElementAtIndex(i).stringValue), mechanicNames.ToArray());
			if (newValue > -1)
				flightModeMechTypeNames.GetArrayElementAtIndex (i).stringValue = mechanicNames[newValue];
		}

		if (GUILayout.Button ("+")) {
			flightModeMechTypeNames.InsertArrayElementAtIndex(0);
		}

		if (GUILayout.Button ("-")) {
			flightModeMechTypeNames.DeleteArrayElementAtIndex(0);
		}

//		EditorGUILayout.PropertyField (mode.FindPropertyRelative ("defaultMechanicName"));
		SerializedProperty modeMechs = mode.FindPropertyRelative ("flightMechanics");
		foreach (string mechName in mechanicNames) {
//			Debug.Log (mechName);
			EditorGUILayout.PropertyField(modeMechs.FindPropertyRelative(mechName.ToLower()), true);
		}

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
