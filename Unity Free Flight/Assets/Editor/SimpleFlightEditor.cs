using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SimpleFlight)), CanEditMultipleObjects]
public class SimpleFlightEditor : Editor {


	public override void OnInspectorGUI() {
		SimpleFlight sf = (SimpleFlight)target;
//		FlightBody fb = sf.fBody;

		sf.togglePhysicsMenu = EditorGUILayout.Toggle ("Physics Menu", sf.togglePhysicsMenu);
		sf.toggleStatsMenu = EditorGUILayout.Toggle ("Stats Menu", sf.toggleStatsMenu);
		sf.toggleLift = EditorGUILayout.Toggle ("Lift", sf.toggleLift);
		sf.toggleDrag = EditorGUILayout.Toggle ("Drag", sf.toggleDrag);
		sf.toggleGravity = EditorGUILayout.Toggle ("Gravity", sf.toggleGravity);

		sf.fBody.unit = (UnitConverter.Units) EditorGUILayout.EnumPopup (sf.fBody.unit);
		sf.fBody.Preset = (FlightBody.Presets)EditorGUILayout.EnumPopup (sf.fBody.Preset);
		sf.fBody.WingSpan = EditorGUILayout.FloatField ("Wing Span", sf.fBody.WingSpan);
		sf.fBody.WingChord = EditorGUILayout.FloatField ("Wing Chord", sf.fBody.WingChord);
		sf.fBody.WingArea = EditorGUILayout.FloatField ("Wing Area", sf.fBody.WingArea);
		sf.fBody.AspectRatio = EditorGUILayout.Slider ("Aspect Ratio", sf.fBody.AspectRatio, 1, 16);
		if( GUILayout.Button("Align To Wing Dimensions" ) ) 		{sf.fBody.setFromWingDimensions ();}
		if (GUILayout.Button ("Align Dimensions to Wing Area") ) 	{sf.fBody.setDimensionsFromArea ();}
		if (GUILayout.Button ("Align Dimensions to Aspect Ratio")) 	{sf.fBody.setDimensionsFromAR ();}

		//myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
		//EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
	}
}
