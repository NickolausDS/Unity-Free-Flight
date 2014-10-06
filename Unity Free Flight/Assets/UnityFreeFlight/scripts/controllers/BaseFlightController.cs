/*
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

	public FlightMechanics flightPhysics;
	/// <summary>
	/// true if flying, false if on the ground.
	/// </summary>
	public bool isFlying = true;

	//=============
	//Unity Editor-Configurable Settings
	//=============

	//Basic gliding input, values in degrees
	public float maxTurnBank = 45.0f;
	public float maxPitch = 20.0f;
	public float directionalSensitivity = 2.0f;

	public bool enabledFlapping = true;
	public float regularFlaptime = 0.5f;
	public float minimumFlapTime = 0.2f;
	public float flapStrength = 600.0f;
	public float downbeatStrength = 150.0f;

	public bool enabledFlaring = true;
	//The default pitch (x) we rotate to when we do a flare
	public float flareAngle = 70.0f;
	public float flareSpeed = 3.0f;

	public bool enabledDiving = false;
	
	//Max time "standUp" will take to execute.
	public float maxStandUpTime = 2.0f;
	//Speed which "standUp" will correct rotation. 
	public float standUpSpeed = 2.0f;

	//===========
	//USER INPUT
	//===========

	//These protected vars are meant to be directly used or modified by the 
	//child class, and generally read from by the physics model. 
	protected Quaternion _inputDirection;
	[Range(0.0f, 1.0f)]
	protected float _inputLeftWingExposure = 1.0f;
	[Range(0.0f, 1.0f)]
	protected float _inputRightWingExposure = 1.0f;
	public float LeftWingExposure { get { return _inputLeftWingExposure; } }
	public float RightWingExposure { get { return _inputRightWingExposure; } }
	protected int _inputInvertedSetting = -1;
	protected bool _inputFlaring = false;
	protected bool _inputDiving = false;
	protected bool _inputFlap = false;
	protected bool _inputSprint = false;
	[Range(-1.0f, 1.0f)]
	protected float _inputPitch = 0.0f;
	[Range(-1.0f, 1.0f)]
	protected float _inputBank = 0.0f;

	//Even though Inverted as a property here is invisible to the inspector, 
	//using the property in this way makes it convienient to access externally,
	//in order to *toggle* the setting on and off. Expressing _invertedSetting internally
	//an integer makes it very easy to apply to input. 
	public bool Inverted {
		get {
			if (_inputInvertedSetting == 1) 
				return true; 
			return false;
		}
		set {
			if (value == true) 
				_inputInvertedSetting = -1;
			else
				_inputInvertedSetting = 1;
		}
	}

	//=============
	//Unity Events
	//=============

	void Awake() {
		flightPhysics = new FlightMechanics (rigidbody);

	}
	
	/// <summary>
	/// This is where all player input control code goes, once this class is overridden. 
	/// </summary>
	void Update() {
		Debug.LogWarning ("Base Flight Controller is not a valid controller! Please add a different flight controller," +
			"or inherit from this class if you intend to write your own. Offending Game Object: " + gameObject.name);
		this.enabled = false;
	}

	/// <summary>
	/// In relation to Update() this is where we decide how to act on the user input, then
    /// compute the physics and animation accordingly
	/// </summary>
	void FixedUpdate () {

		if (isFlying) {
			applyStandardFlightMechanics();
		}

	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		if (isFlying) {
			isFlying = false;
			StartCoroutine (standUp ());
		}
	}

	//==================
	//Functionality
	//==================


	void applyStandardFlightMechanics() {
		//precedence is as follows: flaring, diving, regular gliding flight. This applies if the
		//player provides multiple inputs. Some mechanics can be performed at the same time, such 
		//as flapping while flaring, or turning while diving. 
		
		if (_inputFlaring && enabledFlaring) {
			//Flare is the same as directional input, except with exagerated pitch and custom speed. 
			flightPhysics.directionalInput(_inputBank * maxTurnBank, _inputPitch * maxPitch - flareAngle, flareSpeed);
			if(_inputFlap && enabledFlapping)
				flightPhysics.flap (minimumFlapTime, regularFlaptime, flapStrength, downbeatStrength, true, false);
		} else if(_inputDiving && enabledDiving) {
			flightPhysics.wingFold(_inputLeftWingExposure, _inputRightWingExposure);
		} else {
			//regular flight
			//find the degree of turning the player wants
			flightPhysics.directionalInput(_inputBank * maxTurnBank, _inputPitch * maxPitch, directionalSensitivity);
			if(_inputFlap && enabledFlapping)
				flightPhysics.flap (minimumFlapTime, regularFlaptime, flapStrength, downbeatStrength, true, false);
		}
		
		flightPhysics.doStandardPhysics ();
			
	}


	/// <summary>
	/// Straightenes the flight object on landing, by rotating the roll and pitch
	/// to zero over time. Public vars "standUpSpeed" and "maxStandUpTime" can 
	/// be used to tweak behaviour.
	/// </summary>
	/// <returns>The up.</returns>
	protected IEnumerator standUp() {

		//Make sure no physics are executed while this happens. 
		//enableNoneMode = true;
		//Dis-allow physics, which prevents 'falling over' before the object can stand
		rigidbody.isKinematic = true;
		//Find the direction the flight object should stand, without any pitch and roll. 
		Quaternion desiredRotation = Quaternion.identity;
		desiredRotation.eulerAngles = new Vector3 (0.0f, transform.rotation.eulerAngles.y, 0.0f);
		//Grab the current time. We don't want 'standUp' to take longer than maxStandUpTime
		float time = Time.time;

		//Break if the player started flying again, or if we've reached the desired rotation (within 5 degrees)
		while (!isFlying && Quaternion.Angle(transform.rotation, desiredRotation) > 5.0f) {
			//Additionally break if we have gone over time
			if (time + maxStandUpTime < Time.time)
				break;
			//Correct the rotation
			transform.rotation = Quaternion.Lerp (transform.rotation, desiredRotation, standUpSpeed * Time.deltaTime);
			yield return null;
		}

		//enableGroundMode = true;

	}


}
