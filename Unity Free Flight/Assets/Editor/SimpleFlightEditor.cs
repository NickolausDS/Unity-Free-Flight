using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SimpleFlight)), CanEditMultipleObjects]
public class SimpleFlightEditor : Editor {


	public override void OnInspectorGUI() {
		SimpleFlight sf = (SimpleFlight)target;

		sf.togglePhysicsMenu = EditorGUILayout.Toggle ("Physics Menu", sf.togglePhysicsMenu);
		sf.toggleStatsMenu = EditorGUILayout.Toggle ("Stats Menu", sf.toggleStatsMenu);
		sf.toggleLift = EditorGUILayout.Toggle ("Lift", sf.toggleLift);
		sf.toggleDrag = EditorGUILayout.Toggle ("Drag", sf.toggleDrag);
		sf.toggleGravity = EditorGUILayout.Toggle ("Gravity", sf.toggleGravity);

		sf.fObj.Unit = (UnitConverter.Units) EditorGUILayout.EnumPopup (sf.fObj.Unit);
		sf.fObj.Preset = (FlightObject.Presets)EditorGUILayout.EnumPopup (sf.fObj.Preset);
		sf.fObj.WingSpan = EditorGUILayout.FloatField ("Wing Span", sf.fObj.WingSpan);
		sf.fObj.WingChord = EditorGUILayout.FloatField ("Wing Chord", sf.fObj.WingChord);
		sf.fObj.WingArea = EditorGUILayout.FloatField ("Wing Area", sf.fObj.WingArea);
		sf.fObj.AspectRatio = EditorGUILayout.Slider ("Aspect Ratio", sf.fObj.AspectRatio, 1, 16);
		sf.fObj.Weight = EditorGUILayout.FloatField ("Weight", sf.fObj.Weight);
		if( GUILayout.Button("Align To Wing Dimensions" ) ) 		{sf.fObj.setFromWingDimensions ();}
		if (GUILayout.Button ("Align Dimensions From Area & AR") ) 	{sf.fObj.setWingDimensions ();}

		//Can't figure this part out. We want to get a controller from the inspector, and convert it to a basecontroller so 
		//the simple flight script knows how to use it. Without this functionality, simpleFlight always defaults to the simple
		//controller
//		MonoScript controllerscript;
//		controllerscript = (MonoScript) EditorGUILayout.ObjectField ("Controller", sf.controller, typeof(MonoScript), false);
//		if (controllerscript)
//						sf.controller = (BaseController) MonoScript.FromMonoBehaviour ((BaseController)controllerscript);

		//myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
		//EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
	}
}
