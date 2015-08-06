using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityFreeFlight;

[CustomEditor (typeof (FreeFlight) )]
public class FreeFlightEditor : Editor {
	
	public override void OnInspectorGUI () {

		serializedObject.Update ();
		EditorGUILayout.PropertyField( serializedObject.FindProperty ("modeManager"));
		serializedObject.ApplyModifiedProperties ();
	}
}
