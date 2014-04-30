/*
 * Base Flight Controller
 * 
 * Note: This is an incomplete controller! It should never be added directly to a
 * gameobject! 
 * 
 * Inherit from this class if you want to write your own controller. Lots of effort was
 * put in to decouple the mechanic from the controller. The functionality is here, while
 * the feel for how it works is left up to the controller. 
 * 
 */


using UnityEngine;
using System.Collections;

public class BaseFlightController : MonoBehaviour {
	
	//The public vars are intended to be modified by the inspector. Anything that
	//can be mod
	public bool flightEnabled = true;

	public bool flappingEnabled = false;
	public float regularFlaptime = 0.7f;
	public float minimumFlapTime = 0.2f;
	public float flapStrength = 400.0f;

	//These protected vars are meant to be directly used or modified by the 
	//child class, and generally read from by the physics model. 
	protected Quaternion _userInput;
	protected int _invertedSetting = -1;

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
//			MonoBehaviour thisobj = gameObject.GetComponent<BaseFlightController>();
//			thisobj.SendMessage("setDefaultController", SendMessageOptions.DontRequireReceiver);
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


}
