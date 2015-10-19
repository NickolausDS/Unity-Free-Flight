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
	[Tooltip ("Disable this component when an uncaught Exception happens")]
	public bool disableOnError = true;
	
	public static string version() {
		return UnityFreeFlight.Version.version ();
	}

	//=============
	//Unity Events
	//=============

	public void Awake() {
		checkAnimationController ();
	}

	public void OnEnable () {
		if (modeManager == null) {
			modeManager = new ModeManager ();
		}
		try {
			modeManager.init (gameObject);
		} catch (Exception e) {
			error (e, "Mechanic Initialization");
		}
	}

	void SwitchModes(MovementModes newmode) {
		try {
			modeManager.switchModes (newmode);
		} catch (Exception e) {
			error (e, "Mode Switch");
		}
	}

	void Start() {
		try {
			modeManager.start ();
		} catch (Exception e) {
			error (e, "Mechanic Start");
		}
	}
	
	/// <summary>
	/// Get input from the player 
	/// </summary>
	void Update() {
		try {
			modeManager.getInputs ();
		} catch (Exception e) {
			error (e, "Mechanic Inputs");
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
			error (e, "Mechanic Fixed Update");
		}
	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		modeManager.switchModes (MovementModes.Ground);
	}

	private void error(Exception e, string section) {
		Debug.LogError (string.Format("({0} -- Free Flight Component): Exception in {1}: [{2}] {3} \n{4}", 
		                              gameObject.name, section, e.GetType().ToString(), e.Message, e.StackTrace.ToString()));
		if (disableOnError)
			enabled = false;
	}

	private void checkAnimationController() {
		Animator animator = gameObject.GetComponentInChildren<Animator> ();
		if (animator == null)
			Debug.LogWarning (string.Format(
				"An Animator needs to be attached to '{0}' or one of its children", gameObject.name));
	}

}
