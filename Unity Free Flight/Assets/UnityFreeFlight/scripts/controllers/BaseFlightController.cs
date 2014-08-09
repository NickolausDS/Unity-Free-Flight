﻿/*
 * Base Flight Controller
 * 
 * Note: This is an incomplete controller! It should never be added directly to a
 * gameobject! 
 * 
 * Inherit from this class if you want to write your own controller. Lots of effort was
 * put in to decouple the mechanic from the controller. The functionality is here, while
 * the feel for how it works is left up to the controller. If you find yourself writing
 * complicated controllers, check here to make sure the functionality you need doesn't 
 * already exist here.
 * 
 */


using UnityEngine;
using System.Collections;

[RequireComponent (typeof(FreeFlight))]
public class BaseFlightController : MonoBehaviour {
	
	//The public vars are intended to be modified by the inspector. Anything that
	//can be mod
	public bool flightEnabled = true;

	public bool flappingEnabled = false;
	public float regularFlaptime = 0.5f;
	public float minimumFlapTime = 0.2f;
	public float flapStrength = 600.0f;
	public float downbeatStrength = 150.0f;

	public bool flaringEnabled = false;

	//The default pitch (x) we rotate to when we do a flare
	public float defaultFlareAngle = 30.0f;
	public float flareSpeed = 3.0f;

	public bool divingEnabled = false;
	public bool thrustingEnabled = false;

	//These protected vars are meant to be directly used or modified by the 
	//child class, and generally read from by the physics model. 
	protected Quaternion _userInput;
	[Range(0.0f, 1.0f)]
	protected float _leftWingExposure = 1.0f;
	[Range(0.0f, 1.0f)]
	protected float _rightWingExposure = 1.0f;
	protected int _invertedSetting = -1;
	//Even though Inverted as a property here is invisible to the inspector, 
	//using the property in this way makes it convienient to access externally,
	//in order to *toggle* the setting on and off. Expressing _invertedSetting internally
	//an integer makes it very easy to apply to input. 
	public bool Inverted {
		get {
			if (_invertedSetting == 1) 
				return true; 
			return false;
		}
		set {
			if (value == true) 
				_invertedSetting = -1;
			else
				_invertedSetting = 1;
		}
	}

	//Property to get the user rotation
	public Quaternion UserInput { get { return _userInput; } }
	public float LeftWingExposure { get { return _leftWingExposure; } }
	public float RightWingExposure { get { return _rightWingExposure; } }

	//These are checked by Free Flight before executing flight physics, and cause it
	//to change states. They are true only when the state needs to change, and revert
	//back to false after they are checked. 
	protected bool enableGroundMode = false;
	protected bool enableFlightMode = false;
	protected bool enableNoneMode = false;
	public bool EnableGroundMode { get { bool ret = enableGroundMode; enableGroundMode = false; return ret; } }
	public bool EnableFlightMode { get { bool ret = enableFlightMode; enableFlightMode = false; return ret; } }
	public bool EnableNoneMode { get { bool ret = enableNoneMode; enableNoneMode = false; return ret; } }

	//Private vars, meant only for the Base Flight Controller.
	private bool _hasWarnedUser = false;
	
	//These track states for two separate kinds of flaps the player can do. The states are executed by
	//Flight Mechanics
	private bool regularFlap = false;
	private bool quickFlap = false;
	public bool RegularFlap { get { bool ret = regularFlap; regularFlap = false; return ret; } }
	public bool QuickFlap { get { bool ret = quickFlap; quickFlap = false; return ret; } }
	
	//The current flare angle. This may change based on user input
	protected float flareAngle = 30.0f;
	protected bool startFlare = false;
	protected bool isFlaring = false;
	protected bool releaseFlare = false;
	public float FlareAngle { get { return flareAngle; } }
	/// <summary>
	/// 	Returns true if user presses flare button.
	/// </summary>
	/// <value><c>true</c> if do flare; otherwise, <c>false</c>.</value>
	public bool StartFlare { get { bool ret = startFlare; startFlare = false; return ret; } }
	/// <summary>
	/// 	Returns true while we are still flaring, including both the 'doflare'
	/// 	state and the 'releaseFlare' state.
	/// </summary>
	/// <value><c>true</c> if this instance is flaring; otherwise, <c>false</c>.</value>
	public bool IsFlaring { get { return isFlaring; } }
	/// <summary>
	/// 	Returns true if the user released the flare button
	/// </summary>
	/// <value><c>true</c> if release flare; otherwise, <c>false</c>.</value>
	public bool ReleaseFlare { get { return releaseFlare; } }

	//Max time "standUp" will take to execute.
	public float maxStandUpTime = 2.0f;
	//Speed which "standUp" will correct rotation. 
	public float standUpSpeed = 2.0f;


	//We do the warning here, since Update() is really the only method that needs to be overridden.
	//The child class should put all user controls in this method (and not fixedUpdate(), since we're
	//not doing any physics).
	void Update() {
		if (!_hasWarnedUser) {
			_hasWarnedUser = true;
			Debug.LogWarning ("Base Flight Controller is not a valid controller! Please add a different flight controller," +
				"or inherit from this class if you intend to write your own. Offending Game Object: " + gameObject.name);
		}
	}

	/// <summary>
	/// Straightenes the flight object on landing, by rotating the roll and pitch
	/// to zero over time. Public vars "standUpSpeed" and "maxStandUpTime" can 
	/// be used to tweak behaviour.
	/// </summary>
	/// <returns>The up.</returns>
	protected IEnumerator standUp() {

		//Make sure no physics are executed while this happens. 
		enableNoneMode = true;
		//Dis-allow physics, which prevents 'falling over' before the object can stand
		rigidbody.isKinematic = true;
		//Find the direction the flight object should stand, without any pitch and roll. 
		Quaternion desiredRotation = Quaternion.identity;
		desiredRotation.eulerAngles = new Vector3 (0.0f, transform.rotation.eulerAngles.y, 0.0f);
		//Grab the current time. We don't want 'standUp' to take longer than maxStandUpTime
		float time = Time.time;

		//Break if the player started flying again, or if we've reached the desired rotation (within 5 degrees)
		while (!flightEnabled && Quaternion.Angle(transform.rotation, desiredRotation) > 5.0f) {
			//Additionally break if we have gone over time
			if (time + maxStandUpTime < Time.time)
				break;
			//Correct the rotation
			transform.rotation = Quaternion.Lerp (transform.rotation, desiredRotation, standUpSpeed * Time.deltaTime);
			yield return null;
		}

		enableGroundMode = true;

	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		if (flightEnabled) {
			flightEnabled = false;
			StartCoroutine (standUp ());
		}
	}
	

	/// <summary>
	///Flap the wings to gain altitude. Flapping wings while rotated in any direction will 
	///instead cause momentum in the opposite direction (think of flapping to slow down).
	///
	///Flapping has two modes, regular and interrupted. Regular is designed for flaps only
	///at regular intervals, and makes sense when the player holds down the 'flap' button. 
	///Quick flapping allows the player to flap faster than regular, and makes sense
	///when the user mashes the 'flap' button in quick succession. 
	/// </summary>
	/// <param name="isQuickFlap">If set to <c>true</c> quick flap.</param>
	public void flapWings(bool isQuickFlap = false) {

		if (flappingEnabled) {
			regularFlap = true;
			quickFlap = isQuickFlap;
		}

	}

	//A flare is a position that birds, AND aircraft will take upon landing. It's characterized by
	//maximizing wing area with a very high angle of attack (pitch). It's typical for birds to 
	//enter such a high angle of attack that they stall their wings in the process.
	public void flare(float deltaAngle, bool release) {
		if (flaringEnabled) {
			if (!release && !isFlaring && !startFlare) {
				startFlare = true;
				isFlaring = true;
			} else if (release && isFlaring) {
				releaseFlare = true;
			} else if (isFlaring) {
				flareAngle += deltaAngle;
			}
		}
	}

	public void terminateFlare() {
		startFlare = false;
		isFlaring = false;
		releaseFlare = false;
	}

	//A dive includes folding either one or both of the wings, increasing speed and decent rate.
	public void dive(float leftWingExposureAmount, float rightWingExposureAmount) {
		_leftWingExposure = leftWingExposureAmount;
		_rightWingExposure = rightWingExposureAmount;
	}

	public void dive(bool left, bool right) {
		if (left) {
			_leftWingExposure = 0.0f;
		} else {
			_leftWingExposure = 1.0f;
		}

		if (right) {
			_rightWingExposure = 0.0f;
		} else {
			_rightWingExposure = 1.0f;
		}

	}

	//Add raw thrust to the flight object
	//Birds usually can't do this, unless they've eaten beans and are feeling tootie.
	public void thrust(float power) {
		if (thrustingEnabled) {
			//I'm so embarrased! *Blush*
			Debug.LogWarning("Thrusting not implemented yet!");
		}
	}



}
