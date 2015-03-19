using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityFreeFlight;

[CustomPropertyDrawer (typeof(ModeManager))]
public class ModeManagerDrawer : PropertyDrawer {

	bool flightModeFoldout;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		EditorGUILayout.PropertyField (property.FindPropertyRelative("_activeMode"));

		//TODO Ground mode still uses a generic property field, as it hasn't been refactored yet.
		EditorGUILayout.PropertyField (property.FindPropertyRelative ("groundMode"), true);


		flightModeFoldout = EditorPrefs.GetBool ("flightModeFoldout");
		if(flightModeFoldout = EditorGUILayout.Foldout (flightModeFoldout, "Flight Mode"))
			displayMode (position, property.FindPropertyRelative ("flightMode"), label);
		EditorPrefs.SetBool ("flightModeFoldout", flightModeFoldout);
	}


	public void displayMode(Rect position, SerializedProperty mode, GUIContent label) {
		EditorGUILayout.PropertyField (mode.FindPropertyRelative ("alwaysApplyPhysics"));

		List<string> mechanicNames = getMechanicNames ();
		SerializedProperty modeMechs = mode.FindPropertyRelative ("flightMechanics");


		EditorGUILayout.LabelField ("Default Mechanic:");
		EditorGUILayout.BeginHorizontal ();
		EditorGUI.indentLevel++;
		SerializedProperty flightModeDefaultMechanicName = mode.FindPropertyRelative ("defaultMechanicTypeName");
		int theNewValue = EditorGUILayout.Popup(mechanicNames.IndexOf (
			flightModeDefaultMechanicName.stringValue), mechanicNames.ToArray());
		if (theNewValue > -1)
			flightModeDefaultMechanicName.stringValue = mechanicNames[theNewValue];
		EditorGUI.indentLevel--;
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.LabelField ("Mechanics");
		EditorGUI.indentLevel++;
		SerializedProperty flightModeMechTypeNames = mode.FindPropertyRelative ("mechanicTypeNames");
		for (int i = 0; i < flightModeMechTypeNames.arraySize; i++) {
			EditorGUILayout.BeginHorizontal();

			int newValue = EditorGUILayout.Popup(mechanicNames.IndexOf (
				flightModeMechTypeNames.GetArrayElementAtIndex(i).stringValue), mechanicNames.ToArray());
			if (newValue > -1)
				flightModeMechTypeNames.GetArrayElementAtIndex (i).stringValue = mechanicNames[newValue];

			if (GUILayout.Button ('\u2193'.ToString()))
				flightModeMechTypeNames.MoveArrayElement(i, i+1);
			if (GUILayout.Button ("+")) 
				flightModeMechTypeNames.InsertArrayElementAtIndex(i);
			if (GUILayout.Button ("-")) 
				flightModeMechTypeNames.DeleteArrayElementAtIndex(i);
			EditorGUILayout.PropertyField(modeMechs.FindPropertyRelative(flightModeMechTypeNames.GetArrayElementAtIndex(i).stringValue.ToLower())
			                              .FindPropertyRelative("enabled"));

			EditorGUILayout.EndHorizontal();
		}
		EditorGUI.indentLevel--;


		EditorGUILayout.LabelField ("Mechanics Configuration");
		EditorGUI.indentLevel+=1;
		foreach (string mechName in mechanicNames) {
			EditorGUILayout.PropertyField(modeMechs.FindPropertyRelative(mechName.ToLower()), true);
		}
		EditorGUI.indentLevel-=1;

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
