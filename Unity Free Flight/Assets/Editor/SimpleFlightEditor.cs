using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SimpleFlight)), CanEditMultipleObjects]
public class SimpleFlightEditor : Editor {

	MonoBehaviour groundController = null;
	MonoScript tempgc;
	string cname = null;


	public override void OnInspectorGUI() {
		SimpleFlight sf = (SimpleFlight)target;
		GameObject go = sf.gameObject;

//		sf.togglePhysicsMenu = EditorGUILayout.Toggle ("Physics Menu", sf.togglePhysicsMenu);
//		sf.toggleStatsMenu = EditorGUILayout.Toggle ("Stats Menu", sf.toggleStatsMenu);
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

		sf.flightController = go.GetComponent<BaseFlightController> ();
		EditorGUILayout.ObjectField ("Flight Controller", sf.flightController, typeof(MonoBehaviour), false);
		tempgc = (MonoScript) EditorGUILayout.ObjectField ("Ground Controller", tempgc, typeof(MonoScript), false);
		sf.GroundMode = EditorGUILayout.Toggle ("Ground Controller Enabled", sf.GroundMode);
		//		EditorGUILayout.ObjectField ("Ground Controller", groundController, typeof(MonoBehaviour), false);

		if (cname == null && tempgc)
			cname = tempgc.name;
			if (cname != null && !go.GetComponent(cname)) {
				go.AddComponent(cname);
				groundController = (MonoBehaviour) go.GetComponent (cname);
				if (groundController) {
					groundController.enabled = false;
					CharacterController cc = go.GetComponent<CharacterController>();
					if (cc)
						cc.enabled = false;
				}
		}

		groundController = (MonoBehaviour) go.GetComponent (cname);
		if (groundController) {
			sf.groundController = groundController;
		}

	}
}
