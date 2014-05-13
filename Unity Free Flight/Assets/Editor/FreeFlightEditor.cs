using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FreeFlight)), CanEditMultipleObjects]
public class FreeFlightEditor : Editor {

	//All used for setting the ground controller. 
	//It's more complicated than it should be, see
	//setting ground controllers below
	MonoBehaviour groundController = null;
	MonoScript tempgc;
	string cname = null;

	//vars for foldouts
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
			

			tempgc = (MonoScript) EditorGUILayout.ObjectField ("Ground Controller", tempgc, typeof(MonoScript), false);
			setGroundController(ff, go);
			//Used for debugging
//			EditorGUILayout.ObjectField ("Ground Controller", groundController, typeof(MonoBehaviour), false);
//			EditorGUILayout.LabelField(cname);

		}


	}

	//As you may notice, setting ground controllers are a complicated mess.
	//The problem lies with adding the component to the game object. Every time
	//that happens, the monoscript for 'tempgc' mysteriously disappears. I've done
	//hours of research and have yet to figure out why this happens. 
	//
	//We circumvent the problem by using a string 'cname' to keep track of the class
	//when it disappears. 
	private void setGroundController(FreeFlight ff, GameObject go) {

		if (cname == null && tempgc)
			cname = tempgc.name;
		if (cname != null && cname != "" && !go.GetComponent(cname)) {
			groundController = (MonoBehaviour) go.AddComponent(cname);
			if (groundController) {
				groundController.enabled = false;
				CharacterController cc = go.GetComponent<CharacterController>();
				if (cc)
					cc.enabled = false;
			}
		}

		//if the new ground controller is different, swap it.
		groundController = (MonoBehaviour) go.GetComponent (cname);
		if (groundController) {
			if (ff.groundController && ff.groundController.name != groundController.name) {
				Destroy (ff.groundController);
			}
			ff.groundController = groundController;
		}

	}
}
