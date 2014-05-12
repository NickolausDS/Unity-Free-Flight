using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FreeFlight)), CanEditMultipleObjects]
public class FreeFlightEditor : Editor {

	MonoBehaviour groundController = null;

	MonoScript tempgc;
	string cname = null;

	private bool _showPhysicsAttrs = false;
	private bool _showDimensionAttrs = false;
	private bool _showControllers = true;


	public override void OnInspectorGUI() {
		FreeFlight ff = (FreeFlight)target;
		FlightPhysics fo = ff.PhysicsObject;
		GameObject go = ff.gameObject;

		_showPhysicsAttrs = EditorGUILayout.Foldout(_showPhysicsAttrs, "Toggle Physics");
		if (_showPhysicsAttrs) {
			fo.liftEnabled = EditorGUILayout.Toggle ("Lift", fo.liftEnabled);
			fo.dragEnabled = EditorGUILayout.Toggle ("Drag", fo.dragEnabled);
			fo.gravityEnabled = EditorGUILayout.Toggle ("Gravity", fo.gravityEnabled);
		}

		_showDimensionAttrs = EditorGUILayout.Foldout (_showDimensionAttrs, "Wing Dimensions");
		if (_showDimensionAttrs) {
			fo.Unit = (UnitConverter.Units) EditorGUILayout.EnumPopup (fo.Unit);
			fo.Preset = (FlightObject.Presets)EditorGUILayout.EnumPopup (fo.Preset);
			fo.WingSpan = EditorGUILayout.FloatField ("Wing Span (" + fo.getLengthType() + ")", fo.WingSpan);
			fo.WingChord = EditorGUILayout.FloatField ("Wing Chord (" + fo.getLengthType() + ")", fo.WingChord);
			fo.WingArea = EditorGUILayout.FloatField ("Wing Area (" + fo.getAreaType() + ")", fo.WingArea);
			fo.AspectRatio = EditorGUILayout.Slider ("Aspect Ratio", fo.AspectRatio, 1, 16);
			fo.Weight = EditorGUILayout.FloatField ("Weight (" + fo.getWeightType() + ")", fo.Weight);
			if( GUILayout.Button("Align To Wing Dimensions" ) ) 		{fo.setFromWingDimensions ();}
			if (GUILayout.Button ("Align Dimensions From Area & AR") ) 	{fo.setWingDimensions ();}
		}

		_showControllers = EditorGUILayout.Foldout (_showControllers, "Controllers");
		if (_showControllers) {
			ff.flightController = go.GetComponent<BaseFlightController> ();
			EditorGUILayout.ObjectField ("Flight Controller", ff.flightController, typeof(BaseFlightController), false);
			ff.Mode = (FreeFlight.Modes) EditorGUILayout.EnumPopup ("Flight Mode", ff.Mode);
			

			if (ff.groundController)
				ff.groundController = (MonoBehaviour) EditorGUILayout.ObjectField("Ground Controller", ff.groundController, typeof(MonoBehaviour), false);
			else
				tempgc = (MonoScript) EditorGUILayout.ObjectField ("Ground Controller", tempgc, typeof(MonoScript), false);
			//		EditorGUILayout.ObjectField ("Ground Controller", groundController, typeof(MonoBehaviour), false);
		}


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
			if (ff.groundController && ff.groundController.name != groundController.name) {
				Destroy (ff.groundController);
			}
			ff.groundController = groundController;
		}

	}
}
