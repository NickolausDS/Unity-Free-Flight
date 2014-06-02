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
	
	//The public vars are intended to be modified by the inspector. Anything that
	//can be mod
	public bool flightEnabled = true;

	public bool flappingEnabled = false;
	public float regularFlaptime = 0.5f;
	public float minimumFlapTime = 0.2f;
	public float flapStrength = 600.0f;

	public bool flaringEnabled = false;
	public bool divingEnabled = false;
	public bool thrustingEnabled = false;

	//These protected vars are meant to be directly used or modified by the 
	//child class, and generally read from by the physics model. 
	protected Quaternion _userInput;
	[Range(0.0f, 1.0f)]
	protected float _leftWingExposure;
	[Range(0.0f, 1.0f)]
	protected float _rightWingExposure;
	protected int _invertedSetting = -1;
	//These are checked by Free Flight every fixed update and control
	//whether we should activate either mode.
	protected bool enableGround = false;
	protected bool enableFlight = false;

	//Even though Inverted as a property here is invisible to the inspector, 
	//using the property in this way makes it convienient to access via a menu,
	//in order to *toggle* the setting on and off.
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
	//Returns it's value, then sets itself back to false
	public bool EnableGround { get { bool ret = enableGround; enableGround = false; return ret; } }
	public bool EnableFlight { get { bool ret = enableFlight; enableFlight = false; return ret; } }

	//Private vars, meant only for the Base Flight Controller.
	private bool _hasWarnedUser = false;
	//Private flapping vars
	private bool flapping;
	private bool wingsHaveFlappedInDownPosition;
	private float currentFlaptime;
	

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

	//Default behavior used to transition from flight to ground. You're welcome to 
	//override this in a controller to change the default behaviour.
	protected IEnumerator standUp() {
		Quaternion rot;
		if (transform.rotation.eulerAngles.x != 0.0f && transform.rotation.eulerAngles.z != 0.0f)
			rot = Quaternion.LookRotation (new Vector3(transform.rotation.eulerAngles.x, 0.0f, transform.rotation.eulerAngles.z));
		else
			rot = Quaternion.identity;
		float maxTime = 1.0f;
		transform.Translate (0.0f, 1.0f, 0.0f);
		
		while (!flightEnabled && maxTime > 0.0f && (Mathf.Abs (transform.rotation.eulerAngles.x) > 1.0f || Mathf.Abs (transform.rotation.eulerAngles.z) > 1.0f)) {
			transform.rotation = Quaternion.Lerp (transform.rotation, rot, 2.0f * Time.deltaTime);
			maxTime -= Time.deltaTime;
			yield return null;
		}
	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		if (flightEnabled) {
			enableGround = true;
			flightEnabled = false;
			StartCoroutine (standUp ());
		}
	}

	//Flap the wings to gain altitude. Flapping wings while rotated in any direction will 
	//instead cause momentum in the opposite direction (think of flapping to slow down).
	//
	//Flapping has two modes, regular and interrupted. Regular is designed for flaps only
	//at regular intervals, and makes sense when the player holds down the 'flap' button. 
	//Interrupted flapping allows the player to flap faster than regular, and makes sense
	//when the user mashes the 'flap' button in quick succession. 
	public void flapWings(bool interruptFlap = false) {

		if (flappingEnabled) {

			//We can only flap if we're not currently flapping, or the user triggered
			//an 'interruptFlap', which just means "We're flapping faster than the regular
			//flap speed." InterruptFlaps are usually triggered by the user mashing the flap
			//button rather than just holding it down.
			if (!flapping || (interruptFlap && currentFlaptime > minimumFlapTime)) {
				flapping = true;
				currentFlaptime = 0.0f;
				rigidbody.AddForce (new Vector3 (0, flapStrength, 0));
			}

			//Here we deal with flapping at a regular interval. It may be better to use something
			//other than time.deltatime, it may give us incorrect readings
			if (flapping) {
				currentFlaptime += Time.deltaTime;
				if (currentFlaptime > regularFlaptime * 0.9f && !wingsHaveFlappedInDownPosition) {
					rigidbody.AddForce( new Vector3 (0, -flapStrength/4, 0));
					wingsHaveFlappedInDownPosition = true;
				} else if (currentFlaptime > regularFlaptime) {
					flapping = false;
					wingsHaveFlappedInDownPosition = false;
				}
			}
		}

	}

	//A flare is a position that birds, AND aircraft will take upon landing. It's characterized by
	//maximizing wing area with a very high angle of attack (pitch). It's typical for birds to 
	//enter such a high angle of attack that they stall their wings in the process.
	public void flare() {
		if (flaringEnabled) {
			Debug.LogWarning("Flarring not implemented yet!");
		}
	}

	//A dive includes folding either one or both of the wings, increasing speed and decent rate.
	public void dive(float leftWingPercentFold, float rightWingPercentFold) {
		if (divingEnabled) {
			Debug.LogWarning("Diving not implemented yet!");
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
