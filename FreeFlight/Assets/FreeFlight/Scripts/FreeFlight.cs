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
			Debug.Log ("Instantiating Free Flight component");
		}
//		Debug.Log ("Initializing...");
		modeManager.init (gameObject);
	}

	void SwitchModes(MovementModes newmode) {
		modeManager.switchModes (newmode);
	}

	void Start() {
		modeManager.start ();
	}
	
	/// <summary>
	/// Get input from the player 
	/// </summary>
	void Update() {
		modeManager.getInputs ();
	}
	
	/// <summary>
	/// In relation to Update() this is where we decide how to act on the user input, then
	/// compute the physics and animation accordingly
	/// </summary>
	void FixedUpdate () {	
		modeManager.applyInputs ();
	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		modeManager.switchModes (MovementModes.Ground);
	}


}
