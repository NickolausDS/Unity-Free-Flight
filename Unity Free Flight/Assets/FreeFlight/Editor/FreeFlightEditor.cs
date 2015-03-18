using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityFreeFlight;

[CustomEditor (typeof (FreeFlight) )]
[CanEditMultipleObjects]
public class FreeFlightEditor : Editor {

	FreeFlight freeFlight;
//	int ListSize;
	
	void OnEnable(){
//		freeFlight = (FreeFlight)target;
//		freeFlight.OnEnable ();
	}
	

	public override void OnInspectorGUI () {
//		List<string> mechanicNames = t.modeManager.flightMode.mechanicNames;
//		List<Mechanic> mechanics = t.modeManager.flightMode.mechanics;
//		List<Type> tlist = UnityTypeResolver.GetAllSubTypes (typeof(Mechanic));
//
//
//		while (mechanicNames.Count > mechanics.Count) { mechanicNames.RemoveAt(mechanicNames.Count-1);}
//		while (mechanicNames.Count < mechanics.Count) { mechanicNames.Add(typeof(Mechanic).Name);}
//
//		//Look through the names of mechanics, and make sure the types match. If the types don't match, 
//		//create a new instance of the correct type. 
//		for (int i = 0; i < mechanicNames.Count; i++) {
//			if (string.Compare (mechanicNames[i], mechanics[i].GetType().Name) != 0) {
//				mechanics[i] = (Mechanic) Activator.CreateInstance (getTypeByName (tlist, mechanicNames[i]));
//				Debug.Log (string.Format ("At index {0}: Found {1} type, Replacing with {2} type.",
//				                          i, mechanicNames[i], getTypeByName(tlist, mechanicNames[i]).Name));
//				t.OnEnable();
//			}
//		}


		serializedObject.Update ();
//		freeFlight = (FreeFlight)target;
//		freeFlight.OnEnable ();
		EditorGUILayout.PropertyField( serializedObject.FindProperty ("modeManager"), true);
//		freeFlight = (FreeFlight)target;
//		setupMechanics (freeFlight.modeManager.flightMode);
		serializedObject.ApplyModifiedProperties ();

	}
	

	public Type getTypeByName(List<Type> list, string s) {
		foreach (Type t in list) {
			if (string.Compare (t.Name, s) == 0)
				return t;
		}
		return null;
	}
}
