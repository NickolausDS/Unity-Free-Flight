using UnityEngine;
using UnityEngine.Internal;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityFreeFlight;
using System.Reflection;

/// <summary>
/// Free Flight -- a Unity Component that adds flight to any unity object. 
/// </summary>
[RequireComponent (typeof(Rigidbody))]
[assembly:AssemblyVersion ("0.5.0")]
public class FreeFlight : MonoBehaviour {
	
	public ModeManager modeManager;

	//=============
	//Unity Events
	//=============

	public void OnEnable () {
		if (modeManager == null) {
			modeManager = new ModeManager ();
		}
		try {
			modeManager.init (gameObject);
		} catch (Exception e) {
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script Failed Initialization:\n{1}", gameObject.name, e.StackTrace.ToString()));
			enabled = false;
		}
	}

	void SwitchModes(MovementModes newmode) {
		try {
			modeManager.switchModes (newmode);
		} catch (Exception e) {
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script Failed Switching Modes:\n{1}", gameObject.name, e.StackTrace.ToString()));
			enabled = false;
		}
	}

	void Start() {
		try {
			modeManager.start ();
		} catch (Exception e) {
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script failed to start:\n{1}", gameObject.name, e.StackTrace.ToString()));
			enabled = false;
		}
	}
	
	/// <summary>
	/// Get input from the player 
	/// </summary>
	void Update() {
		try {
			modeManager.getInputs ();
		} catch (Exception e) {
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script Failed on Get Inputs:\n{1}", gameObject.name, e.StackTrace.ToString()));
			enabled = false;
		}
	}
	
	/// <summary>
	/// In relation to Update() this is where we decide how to act on the user input, then
	/// compute the physics and animation accordingly
	/// </summary>
	void FixedUpdate () {	
		try {
			modeManager.applyInputs ();
		} catch (Exception e) {
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script Failed on Applying Inputs:\n{1}", gameObject.name, e.StackTrace.ToString()));
			enabled = false;
		}
	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		modeManager.switchModes (MovementModes.Ground);
	}


}
