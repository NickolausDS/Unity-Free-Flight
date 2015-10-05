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
public class FreeFlight : MonoBehaviour {
	
	public ModeManager modeManager;

	public static string version() {
		return UnityFreeFlight.Version.version ();
	}

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
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script Failed Initialization: [{1}] {2} \n{3}", 
			                              gameObject.name, e.GetType().ToString(), e.Message, e.StackTrace.ToString()));
			enabled = false;
		}
	}

	void SwitchModes(MovementModes newmode) {
		try {
			modeManager.switchModes (newmode);
		} catch (Exception e) {
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script Failed Switching Modes: [{1}] {2} \n{3}", 
			                              gameObject.name, e.GetType().ToString(), e.Message, e.StackTrace.ToString()));
			enabled = false;
		}
	}

	void Start() {
		try {
			modeManager.start ();
		} catch (Exception e) {
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script failed to start: [{1}] {2} \n{3}", 
			                              gameObject.name, e.GetType().ToString(), e.Message, e.StackTrace.ToString()));
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
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script Failed on Get Inputs: [{1}] {2} \n{3}", 
			                              gameObject.name, e.GetType().ToString(), e.Message, e.StackTrace.ToString()));
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
			Debug.LogError (string.Format("({0} -- Free Flight Component): Script Failed on Applying Inputs: [{1}] {2} \n{3}", 
			                              gameObject.name, e.GetType().ToString(), e.Message, e.StackTrace.ToString()));
			enabled = false;
		}
	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		modeManager.switchModes (MovementModes.Ground);
	}


}
