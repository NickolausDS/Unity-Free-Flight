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
		FlightPhysics fo = ff.PhysicsObject;
		GameObject go = ff.gameObject;

		fo.liftEnabled = EditorGUILayout.Toggle ("Lift", fo.liftEnabled);
		fo.dragEnabled = EditorGUILayout.Toggle ("Drag", fo.dragEnabled);
		fo.gravityEnabled = EditorGUILayout.Toggle ("Gravity", fo.gravityEnabled);

		fo.Unit = (UnitConverter.Units) EditorGUILayout.EnumPopup (fo.Unit);
		fo.Preset = (FlightObject.Presets)EditorGUILayout.EnumPopup (fo.Preset);
		fo.WingSpan = EditorGUILayout.FloatField ("Wing Span (" + fo.getLengthType() + ")", fo.WingSpan);
		fo.WingChord = EditorGUILayout.FloatField ("Wing Chord (" + fo.getLengthType() + ")", fo.WingChord);
		fo.WingArea = EditorGUILayout.FloatField ("Wing Area (" + fo.getAreaType() + ")", fo.WingArea);
		fo.AspectRatio = EditorGUILayout.Slider ("Aspect Ratio", fo.AspectRatio, 1, 16);
		fo.Weight = EditorGUILayout.FloatField ("Weight (" + fo.getWeightType() + ")", fo.Weight);
		if( GUILayout.Button("Align To Wing Dimensions" ) ) 		{fo.setFromWingDimensions ();}
		if (GUILayout.Button ("Align Dimensions From Area & AR") ) 	{fo.setWingDimensions ();}

		ff.flightController = go.GetComponent<BaseFlightController> ();
		EditorGUILayout.ObjectField ("Flight Controller", ff.flightController, typeof(MonoBehaviour), false);
		tempgc = (MonoScript) EditorGUILayout.ObjectField ("Ground Controller", tempgc, typeof(MonoScript), false);
		ff.GroundMode = EditorGUILayout.Toggle ("Ground Controller Enabled", ff.GroundMode);
		//		EditorGUILayout.ObjectField ("Ground Controller", groundController, typeof(MonoBehaviour), false);

		if (cname == null && tempgc)
			cname = tempgc.name;
			if (cname != null && cname != "" && !go.GetComponent(cname)) {
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
