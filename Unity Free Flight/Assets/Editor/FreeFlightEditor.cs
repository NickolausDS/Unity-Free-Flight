using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(FreeFlight)), CanEditMultipleObjects][SerializeField]
public class FreeFlightEditor : Editor {
	
	//vars for foldouts
	private bool _showFlightControls = true;
	private bool _showGroundControls = false;
	private bool _showTakeoffLanding = false;
	private bool _showPhysics = false;
	private bool _showProperties = false;


	public override void OnInspectorGUI() {
		FreeFlight ff = (FreeFlight) target;
		FreeFlightPhysics fp = ff.flightPhysics;


		if (_showFlightControls = EditorGUILayout.Foldout (_showFlightControls, "Flight Controls")) {
			editorFlightControls(ff);
		}
		if (_showGroundControls = EditorGUILayout.Foldout (_showGroundControls, "Ground Controls")) {
			editorGroundControls(ff);
		}
		if (_showTakeoffLanding = EditorGUILayout.Foldout (_showTakeoffLanding, "Takeoff/Landing Controls")) {
			editorTakeoffLanding(ff);
		}
		if (_showPhysics = EditorGUILayout.Foldout(_showPhysics, "Physics")) {
			editorPhysics(ff, fp);
		}
		if (_showProperties = EditorGUILayout.Foldout (_showProperties, "Wing Properties")) {
			editorWingProperties(fp);
		}
	}

	void editorFlightControls(FreeFlight ff) {
	
		if (ff.enabledGliding = EditorGUILayout.Toggle ("Enabled Gliding", ff.enabledGliding)) {
			ff.maxTurnBank = EditorGUILayout.FloatField ("Turn Bank (Degrees)", ff.maxTurnBank); 
			ff.maxPitch = EditorGUILayout.FloatField ("Pitch (Degrees)", ff.maxPitch); 
			ff.directionalSensitivity = EditorGUILayout.FloatField ("Degrees per Second", ff.directionalSensitivity); 
		}
		if (ff.enabledFlapping = EditorGUILayout.Toggle ("Enabled Flapping", ff.enabledFlapping)) {
			ff.flapSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Flap Sound", ff.flapSoundClip, typeof(AudioClip), false);
			ff.flapStrength = EditorGUILayout.FloatField ("Flap Strength", ff.flapStrength); 

		}		
		if (ff.enabledFlaring = EditorGUILayout.Toggle ("Enabled Flaring", ff.enabledFlaring)) {
			ff.flareAngle = (float) EditorGUILayout.FloatField ("Flare Angle", ff.flareAngle);
			ff.flareSpeed = (float) EditorGUILayout.FloatField ("Flare Speed", ff.flareSpeed);
			ff.flareSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Flare Sound", ff.flareSoundClip, typeof(AudioClip), false);
		}
		if (ff.enabledDiving = EditorGUILayout.Toggle ("Enabled Diving", ff.enabledDiving)) {
			ff.divingSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Dive Sound", ff.divingSoundClip, typeof(AudioClip), false);
		}
		if (ff.enabledWindNoise = EditorGUILayout.Toggle ("Enabled Wind Noise", ff.enabledWindNoise)) {
			ff.windNoiseClip = (AudioClip) EditorGUILayout.ObjectField ("Wind Sound", ff.windNoiseClip, typeof(AudioClip), false);
			ff.windNoiseStartSpeed = (float) EditorGUILayout.FloatField ("Min Speed Start", ff.windNoiseStartSpeed);
			ff.windNoiseMaxSpeed = (float) EditorGUILayout.FloatField ("Max Speed Stop", ff.windNoiseMaxSpeed);

		}
	}

	void editorGroundControls(FreeFlight ff) {
		if (ff.enabledGround = EditorGUILayout.Toggle ("Enabled Ground Locomotion", ff.enabledGround)) {
			ff.walkingNoiseClip = (AudioClip) EditorGUILayout.ObjectField ("Walking Sound", ff.walkingNoiseClip, typeof(AudioClip), false);
			ff.groundDrag = EditorGUILayout.FloatField ("Stopping Speed", ff.groundDrag);
			ff.maxGroundForwardSpeed = (float) EditorGUILayout.FloatField ("Walking Speed", ff.maxGroundForwardSpeed);
			ff.maxGroundTurningDegreesSecond = (float) EditorGUILayout.FloatField ("Turning Speed", ff.maxGroundTurningDegreesSecond);

		}
		if (ff.enabledJumping = EditorGUILayout.Toggle ("Enabled Jumping", ff.enabledJumping)) {
			ff.jumpingNoiseClip = (AudioClip) EditorGUILayout.ObjectField ("Jumping Sound", ff.jumpingNoiseClip, typeof(AudioClip), false);
			ff.jumpHeight = (float) EditorGUILayout.FloatField ("Jump Height", ff.jumpHeight);
		}
	}

	void editorTakeoffLanding(FreeFlight ff) {
		if (ff.enabledTakeoff = EditorGUILayout.Toggle ("Enabled Takeoff", ff.enabledTakeoff)) {
			ff.takeoffSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Takeoff Sound", ff.takeoffSoundClip, typeof(AudioClip), false);
		}
		if (ff.enabledLanding = EditorGUILayout.Toggle ("Enabled Landing", ff.enabledLanding)) {
			ff.landingSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Landing Sound", ff.landingSoundClip, typeof(AudioClip), false);
			ff.standUpSpeed = (float) EditorGUILayout.FloatField ("Stand Up Speed", ff.standUpSpeed);
			ff.maxStandUpTime = (float) EditorGUILayout.FloatField ("Stand Up Time", ff.maxStandUpTime);

		}
		if (ff.enabledCrashing = EditorGUILayout.Toggle ("Enabled Crashing", ff.enabledCrashing)) {
			ff.crashSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Dive Sound", ff.crashSoundClip, typeof(AudioClip), false);
			ff.crashSpeed = (float) EditorGUILayout.FloatField ("Crash Speed", ff.crashSpeed);

		}
	
	
	}

	void editorPhysics(FreeFlight ff, FreeFlightPhysics fp) {
		ff.state = (FreeFlight.FlightState) EditorGUILayout.EnumPopup("Flight State", ff.state);
		ff.applyFlightPhysicsOnGround = EditorGUILayout.Toggle ("Flight Physics on Ground", ff.applyFlightPhysicsOnGround);
		fp.liftEnabled = EditorGUILayout.Toggle ("Lift", fp.liftEnabled);
		fp.dragEnabled = EditorGUILayout.Toggle ("Drag", fp.dragEnabled);
		fp.gravityEnabled = EditorGUILayout.Toggle ("Gravity", fp.gravityEnabled);
	}

	void editorWingProperties(FreeFlightPhysics fp) {
		fp.Unit = (UnitConverter.Units) EditorGUILayout.EnumPopup (fp.Unit);
		fp.Preset = (FlightObject.Presets)EditorGUILayout.EnumPopup (fp.Preset);
		fp.WingSpan = EditorGUILayout.FloatField ("Wing Span (" + fp.getLengthType() + ")", fp.WingSpan);
		fp.WingChord = EditorGUILayout.FloatField ("Wing Chord (" + fp.getLengthType() + ")", fp.WingChord);
		fp.WingArea = EditorGUILayout.FloatField ("Wing Area (" + fp.getAreaType() + ")", fp.WingArea);
		fp.AspectRatio = EditorGUILayout.Slider ("Aspect Ratio", fp.AspectRatio, 1, 16);
		fp.Weight = EditorGUILayout.FloatField ("Weight (" + fp.getWeightType() + ")", fp.Weight);
		if( GUILayout.Button("Align To Wing Dimensions" ) ) 		{fp.setFromWingDimensions ();}
		if (GUILayout.Button ("Align Dimensions From Area & AR") ) 	{fp.setWingDimensions ();}
	}

	void OnFocus() {
		loadEditorPrefs ();
	}

	void OnLostFocus() {
		saveEditorPrefs ();
	}

	void OnDestroy() {
		saveEditorPrefs ();
	}

	void OnEnable () {
		loadEditorPrefs ();
	}

	void OnDisable () {
		saveEditorPrefs ();
	}


	void saveEditorPrefs() {
		EditorPrefs.SetBool ("ShowFlightControls", _showFlightControls);
		EditorPrefs.SetBool ("ShowGroundControls", _showGroundControls);
		EditorPrefs.SetBool ("ShowTakeoffLanding", _showTakeoffLanding);
		EditorPrefs.SetBool ("ShowPhysics", _showPhysics);
		EditorPrefs.SetBool ("ShowProperties", _showProperties);
	}

	void loadEditorPrefs() {
		_showFlightControls = EditorPrefs.GetBool ("ShowFlightControls", _showFlightControls);
		_showGroundControls = EditorPrefs.GetBool ("ShowGroundControls", _showGroundControls);
		_showTakeoffLanding = EditorPrefs.GetBool ("ShowTakeoffLanding", _showTakeoffLanding);
		_showPhysics = EditorPrefs.GetBool ("ShowPhysics", _showPhysics);
		_showProperties = EditorPrefs.GetBool ("ShowProperties", _showProperties);
	}

}
