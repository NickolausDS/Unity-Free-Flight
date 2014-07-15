using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(FreeFlight)), CanEditMultipleObjects]
public class FreeFlightEditor : Editor {

	private bool _shouldCheckAddDefaultCollider = true;

	//vars for foldouts
	private bool _showPhysicsAttrs = false;
	private bool _showDimensionAttrs = false;
	private bool _showControllers = true;
	private bool _showGroundControllers = true;

	private List<MonoScript> gcMasterList = new List<MonoScript>();


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
			ff.setMode();

			_showGroundControllers = EditorGUILayout.Foldout (_showGroundControllers, "Ground Controllers");
			if (_showGroundControllers) {

				buildMasterList(ff);

				for ( int i = 0; i < gcMasterList.Count; i++) {
					gcMasterList[i] = (MonoScript) EditorGUILayout.ObjectField("Script Priority#" + i, gcMasterList[i], typeof(MonoScript), false);
				}

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("+")) gcMasterList.Add (null);
				if (GUILayout.Button("-")) gcMasterList.RemoveAt(gcMasterList.Count-1);
				EditorGUILayout.EndHorizontal();
				List<MonoBehaviour> gcs = new List<MonoBehaviour>();
				MonoBehaviour newMB;
				foreach ( MonoScript scriptItem in gcMasterList) {
					if (scriptItem == null)
						continue;
					newMB = (MonoBehaviour) go.GetComponent(scriptItem.GetClass().ToString() );
					if (newMB == null) {
						Debug.LogWarning(go.name + " does not have component: " + scriptItem.name);
					} else {
						//Debug.Log ("Successfully added " + scriptItem.name);
						gcs.Add (newMB);
					}
				}

				ff.groundControllers = gcs;
			}



			_shouldCheckAddDefaultCollider = EditorGUILayout.Toggle ("Check/Add Collider", _shouldCheckAddDefaultCollider);
			if (_shouldCheckAddDefaultCollider)
				checkAddDefaultCollider(go);
		}


	}

	/// <summary>
	/// 	Make sure the gcMasterList shows correct data. For some reason unknown, Monoscripts disappear and 
	/// 	switch to null at playtime, or when a random inspector event triggers it. The fix is to rebuild
	/// 	the MonoScript List from the MonoBehaviour List (kept in FreeFlight) whenever we detect this
	/// 	happening.
	/// 
	/// 	This method also needs to make sure there is atleast one item in the gcMasterList,
	///     even if it's null. This makes things easier
	/// 	for the developer to understand when viewing the editor
	/// </summary>
	/// <param name="ff">Refrence to free flight object</param>
	private void buildMasterList(FreeFlight ff) {
		//Rebuild list
		if (gcMasterList.Count == 0 && ff.groundControllers.Count > 0) {
			foreach (MonoBehaviour mb in ff.groundControllers) {
				gcMasterList.Add (MonoScript.FromMonoBehaviour(mb));
			}
		//make sure atleast one item
		} else if (gcMasterList.Count < 1) {
			gcMasterList.Add (null);
		} 


	}

	private void checkAddDefaultCollider(GameObject go) {
		Collider[] col = go.GetComponentsInChildren <Collider> ();
		int numRealColliders = col.Length;
		foreach (Collider thing in col) {
			//Character controllers don't count as colliders
			if (thing.GetType() == typeof(CharacterController))
				numRealColliders -= 1;
		}
		if (numRealColliders < 1) {
			Debug.Log (string.Format ("Free Flight: No collider detected on {0}; adding a default one. " +
				"A collider is necessary to determine when we should land, and to keep the object from " +
				"bumping into objects in-flight. You can stop this check from ever happening by unchecking " +
				"'Check/Add Collider' in the Free Flight Editor", go.name));
			CapsuleCollider newCol = go.AddComponent<CapsuleCollider>();
			newCol.radius = 0.5f;
			newCol.height = 2.0f;
		}
	}
}
