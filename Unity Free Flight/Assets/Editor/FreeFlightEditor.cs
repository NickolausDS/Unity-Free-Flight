using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityFreeFlight;

[CustomEditor(typeof(FreeFlight)), CanEditMultipleObjects][SerializeField]
public class FreeFlightEditor : Editor {
	
	//vars for foldouts
	private bool _showFlightControls = true;
	private bool _showGroundControls = false;
	private bool _showPhysics = false;
	private bool _showProperties = false;


	public override void OnInspectorGUI() {
		FreeFlight ff = (FreeFlight) target;
		FlightMode fm = ff.modeManager.flightMode;
		GroundMode gm = ff.modeManager.groundMode;
		CreatureFlightPhysics fp = fm.flightPhysics;



		if (_showFlightControls = EditorGUILayout.Foldout (_showFlightControls, "Flight Controls")) {
			editorFlightControls(fm);
		}
		if (_showGroundControls = EditorGUILayout.Foldout (_showGroundControls, "Ground Controls")) {
			editorGroundControls(gm);
		}
		if (_showPhysics = EditorGUILayout.Foldout(_showPhysics, "Physics")) {
			editorPhysics(ff, fp);
		}
		if (_showProperties = EditorGUILayout.Foldout (_showProperties, "Wing Properties")) {
			editorWingProperties(fp);
		}

		//save the free flight object if the user has made changes
		if (GUI.changed) {
			EditorUtility.SetDirty (ff);
		}
	}

	void editorFlightControls(FlightMode fm) {
	
		if (fm.enabledGliding = EditorGUILayout.Toggle ("Enabled Gliding", fm.enabledGliding)) {
			fm.maxTurnBank = EditorGUILayout.FloatField ("Turn Bank (Degrees)", fm.maxTurnBank); 
			fm.maxPitch = EditorGUILayout.FloatField ("Pitch (Degrees)", fm.maxPitch); 
			fm.directionalSensitivity = EditorGUILayout.FloatField ("Degrees per Second", fm.directionalSensitivity); 
		}
		if (fm.enabledFlapping = EditorGUILayout.Toggle ("Enabled Flapping", fm.enabledFlapping)) {
			fm.flapSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Flap Sound", fm.flapSoundClip, typeof(AudioClip), false);
			fm.flapStrength = EditorGUILayout.FloatField ("Flap Strength", fm.flapStrength); 

		}		
		if (fm.enabledFlaring = EditorGUILayout.Toggle ("Enabled Flaring", fm.enabledFlaring)) {
			fm.flareAngle = (float) EditorGUILayout.FloatField ("Flare Angle", fm.flareAngle);
			fm.flareSpeed = (float) EditorGUILayout.FloatField ("Flare Speed", fm.flareSpeed);
			fm.flareSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Flare Sound", fm.flareSoundClip, typeof(AudioClip), false);
		}
		if (fm.enabledDiving = EditorGUILayout.Toggle ("Enabled Diving", fm.enabledDiving)) {
			fm.divingSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Dive Sound", fm.divingSoundClip, typeof(AudioClip), false);
		}
		if (fm.enabledWindNoise = EditorGUILayout.Toggle ("Enabled Wind Noise", fm.enabledWindNoise)) {
			fm.windNoiseClip = (AudioClip) EditorGUILayout.ObjectField ("Wind Sound", fm.windNoiseClip, typeof(AudioClip), false);
			fm.windNoiseStartSpeed = (float) EditorGUILayout.FloatField ("Min Speed Start", fm.windNoiseStartSpeed);
			fm.windNoiseMaxSpeed = (float) EditorGUILayout.FloatField ("Max Speed Stop", fm.windNoiseMaxSpeed);
		}

		if (fm.enabledLanding = EditorGUILayout.Toggle ("Enabled Landing", fm.enabledLanding)) {
			fm.landingSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Landing Sound", fm.landingSoundClip, typeof(AudioClip), false);
			fm.standUpSpeed = (float) EditorGUILayout.FloatField ("Stand Up Speed", fm.standUpSpeed);
			fm.maxStandUpTime = (float) EditorGUILayout.FloatField ("Stand Up Time", fm.maxStandUpTime);

		}
		if (fm.enabledCrashing = EditorGUILayout.Toggle ("Enabled Crashing", fm.enabledCrashing)) {
			fm.crashSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Dive Sound", fm.crashSoundClip, typeof(AudioClip), false);
			fm.crashSpeed = (float) EditorGUILayout.FloatField ("Crash Speed", fm.crashSpeed);

		}

	}

	void editorGroundControls(GroundMode gm) {
		if (gm.enabledGround = EditorGUILayout.Toggle ("Enabled Ground Locomotion", gm.enabledGround)) {
			gm.walkingNoiseClip = (AudioClip) EditorGUILayout.ObjectField ("Walking Sound", gm.walkingNoiseClip, typeof(AudioClip), false);
			gm.groundDrag = EditorGUILayout.FloatField ("Stopping Speed", gm.groundDrag);
			gm.maxGroundForwardSpeed = (float) EditorGUILayout.FloatField ("Walking Speed", gm.maxGroundForwardSpeed);
			gm.maxGroundTurningDegreesSecond = (float) EditorGUILayout.FloatField ("Turning Speed", gm.maxGroundTurningDegreesSecond);

		}
		if (gm.enabledJumping = EditorGUILayout.Toggle ("Enabled Jumping", gm.enabledJumping)) {
			gm.jumpingNoiseClip = (AudioClip) EditorGUILayout.ObjectField ("Jumping Sound", gm.jumpingNoiseClip, typeof(AudioClip), false);
			gm.jumpHeight = (float) EditorGUILayout.FloatField ("Jump Height", gm.jumpHeight);
		}
		if (gm.enabledTakeoff = EditorGUILayout.Toggle ("Enabled Takeoff", gm.enabledTakeoff)) {
			gm.takeoffSoundClip = (AudioClip) EditorGUILayout.ObjectField ("Takeoff Sound", gm.takeoffSoundClip, typeof(AudioClip), false);
		}
		if (gm.enabledLaunchIfAirborn = EditorGUILayout.Toggle ("Auto-Takeoff If Airborn", gm.enabledLaunchIfAirborn)) {
			gm.minHeightToLaunchIfAirborn = EditorGUILayout.FloatField ("Min Height", gm.minHeightToLaunchIfAirborn);
		}


	}

	void editorPhysics(FreeFlight ff, FreeFlightPhysics fp) {
		ff.modeManager.activeMode = (MovementModes) EditorGUILayout.EnumPopup("Flight Mode", ff.modeManager.activeMode);
//		ff.applyFlightPhysicsOnGround = EditorGUILayout.Toggle ("Flight Physics on Ground", ff.applyFlightPhysicsOnGround);
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
		EditorPrefs.SetBool ("ShowPhysics", _showPhysics);
		EditorPrefs.SetBool ("ShowProperties", _showProperties);
	}

	void loadEditorPrefs() {
		_showFlightControls = EditorPrefs.GetBool ("ShowFlightControls", _showFlightControls);
		_showGroundControls = EditorPrefs.GetBool ("ShowGroundControls", _showGroundControls);
		_showPhysics = EditorPrefs.GetBool ("ShowPhysics", _showPhysics);
		_showProperties = EditorPrefs.GetBool ("ShowProperties", _showProperties);
	}

}
