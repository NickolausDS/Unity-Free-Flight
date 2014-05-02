using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FreeFlight)), CanEditMultipleObjects]
public class FreeFlightEditor : Editor {

	MonoBehaviour groundController = null;
	MonoScript tempgc;
	string cname = null;


	public override void OnInspectorGUI() {
		FreeFlight ff = (FreeFlight)target;
		GameObject go = ff.gameObject;

//		ff.togglePhysicsMenu = EditorGUILayout.Toggle ("Physics Menu", ff.togglePhysicsMenu);
//		ff.toggleStatsMenu = EditorGUILayout.Toggle ("Stats Menu", ff.toggleStatsMenu);
//		ff.toggleLift = EditorGUILayout.Toggle ("Lift", ff.toggleLift);
//		ff.toggleDrag = EditorGUILayout.Toggle ("Drag", ff.toggleDrag);
//		ff.toggleGravity = EditorGUILayout.Toggle ("Gravity", ff.toggleGravity);
//
//		ff.fObj.Unit = (UnitConverter.Units) EditorGUILayout.EnumPopup (ff.fObj.Unit);
//		ff.fObj.Preset = (FlightObject.Presets)EditorGUILayout.EnumPopup (ff.fObj.Preset);
//		ff.fObj.WingSpan = EditorGUILayout.FloatField ("Wing Span", ff.fObj.WingSpan);
//		ff.fObj.WingChord = EditorGUILayout.FloatField ("Wing Chord", ff.fObj.WingChord);
//		ff.fObj.WingArea = EditorGUILayout.FloatField ("Wing Area", ff.fObj.WingArea);
//		ff.fObj.AspectRatio = EditorGUILayout.Slider ("Aspect Ratio", ff.fObj.AspectRatio, 1, 16);
//		ff.fObj.Weight = EditorGUILayout.FloatField ("Weight", ff.fObj.Weight);
//		if( GUILayout.Button("Align To Wing Dimensions" ) ) 		{ff.fObj.setFromWingDimensions ();}
//		if (GUILayout.Button ("Align Dimensions From Area & AR") ) 	{ff.fObj.setWingDimensions ();}

		ff.flightController = go.GetComponent<BaseFlightController> ();
		EditorGUILayout.ObjectField ("Flight Controller", ff.flightController, typeof(MonoBehaviour), false);
		tempgc = (MonoScript) EditorGUILayout.ObjectField ("Ground Controller", tempgc, typeof(MonoScript), false);
		ff.GroundMode = EditorGUILayout.Toggle ("Ground Controller Enabled", ff.GroundMode);
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
			ff.groundController = groundController;
		}

	}
}
