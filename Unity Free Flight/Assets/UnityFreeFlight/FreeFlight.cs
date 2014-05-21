using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class FreeFlight : MonoBehaviour {

	public enum Modes {None, Ground, Flight, Hybrid};
	/*
	 * None -- Set no controller active, no flight physics active
	 * Ground -- Set only ground controller active, no flight physics active
	 * Flight -- Set only flight controller active, flight physics active
	 * Hybrid -- Flight controller disabled, ground controller active, flight physics active
	 */ 
	public BaseFlightController flightController = null;
	public MonoBehaviour groundController = null;
	private Modes _mode;
	public Modes Mode;
	private FlightMechanics _physicsObject;

		
	void Start() {
		setMode ();
		//If the object is in the sky when we start, give it a little push. The physics
		//get weird if something appears in the sky at zero velocity (works, just looks weird). 
		if (_mode == Modes.Flight)
			rigidbody.velocity = new Vector3(0.0f, 0.0f, 20.0f);
		// We don't want the rigidbody to determine our rotation,
		// we will compute that ourselves
		rigidbody.freezeRotation = true;
	}
		
	public FlightMechanics PhysicsObject { 
		get {
			if (_physicsObject == null)
				_physicsObject = new FlightMechanics (rigidbody);
			return _physicsObject; 
			} 
		}

	//Tries to swap to the vairous controller modes.
	//Success if controllers are set up properly, otherwise
	//fails with an error to console and reverts mode to None
	public void setMode() {
		if (_mode == Mode)
			return;
		else {
			_mode = Mode;
			if (_mode == Modes.None) {
				disableFlight();
				disableGround();
			} else if (_mode == Modes.Flight) {
				disableGround();
				if (!enableFlight()) {
					_mode = Modes.None;
				}
			} else if (_mode == Modes.Ground || _mode == Modes.Hybrid) {
				disableFlight ();
				if (!enableGround()) {
					_mode = Modes.None;
				}
			}
		}
	}
		
	void FixedUpdate() {
		setMode ();

		if (_mode == Modes.Flight || _mode == Modes.Hybrid) {
			if (flightController.divingEnabled)
				PhysicsObject.wingFold (flightController.LeftWingExposure, flightController.RightWingExposure);

			PhysicsObject.doStandardPhysics(flightController.UserInput);

		}
	}

	private bool disableGround() {
		CharacterController cc = gameObject.GetComponent<CharacterController> ();
		if (cc) {
			cc.enabled = false;
		}
		if (groundController) {
			groundController.enabled = false;		
			return true;
		}
		return false;
	}

	private bool enableGround() {
		if (groundController) {
			groundController.enabled = true;
			CharacterController cc = gameObject.GetComponent<CharacterController> ();
			if (cc) {
				cc.enabled = true;
			}
			return true;
		}
		Debug.LogError ("Failed to switch " + gameObject.name + " to ground controller. " +
		                "Make sure you have a valid ground controller attached (The generic Character" +
		                " Controller scripts will work as ground controllers).");
		return false;
	}

	private bool disableFlight() {
		if (flightController) {
			flightController.flightEnabled = false;
			return true;
		}
		return false;
	}

	private bool enableFlight() {
		if (flightController) {
			flightController.flightEnabled = true;
			return true;
		}
		Debug.LogError ("Failed to switch " + gameObject.name + " to flight controller. " +
		                "Make sure you have a valid flight controller attached. You can find" +
		                " Flight Controllers under FreeFlight/Scripts/Controllers.");
		return false;
	}

}
