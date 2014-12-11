using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public List<MonoBehaviour> groundControllers = new List<MonoBehaviour> ();
	private Modes _mode;
	public Modes Mode;
	private FlightMechanics _physicsObject;

		
	void Start() {
		//setMode ();
		//If the object is in the sky when we start, give it a little push. The physics
		//get weird if something appears in the sky at zero velocity (works, just looks weird). 
		if (_mode == Modes.Flight)
			rigidbody.velocity = new Vector3(0.0f, 0.0f, 20.0f);
	}
		
	public FlightMechanics PhysicsObject { 
		get {
			if (_physicsObject == null)
				_physicsObject = new FlightMechanics (rigidbody);
			return _physicsObject; 
			} 
		}

	public BaseFlightController FlightController {
		get {
			if (flightController == null) {
				Debug.LogError ("Free Flight: No Flight Controller attached. Please add" +
				                "a flight controller to enable flight.");
				this.enabled = false;
				return null;
			}
			return flightController;
		}
		set {
			if(value != flightController) {
				Destroy (flightController);
				flightController = value;
			}
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
					Mode = Modes.None;
				}
			} else if (_mode == Modes.Ground || _mode == Modes.Hybrid) {
				disableFlight ();
				if (!enableGround()) {
					_mode = Modes.None;
					Mode = Modes.None;
				}
			}
		}
	}



	/// <summary>
	/// Where user input meets physics. This method is responsible for taking 
	/// user input from the controller (regular Update) and processing it through
	/// the physics pipeline (through fixed update). 
	/// 
	///NOTE: Hybrid mode doesn't work since the introduction of stalling
	///When we're sitting on the ground at zero velocity, flight-physics
	///thinks we're in stall (and executes stall rotations)
	/// </summary>
	void FixedUpdate() {
//		//This is where we detect and change flightmodes. 
//		if (FlightController.EnableNoneMode)
//			Mode = Modes.None;
//		else if (FlightController.EnableFlightMode)
//			Mode = Modes.Flight;
//		else if (FlightController.EnableGroundMode)
//			Mode = Modes.Ground;
//
//		//Update mode if it has changed
//		setMode ();
//
//		//Do flight physics if its enabled
//		if (_mode == Modes.Flight || _mode == Modes.Hybrid) {
//			PhysicsObject.execute(FlightController);
//		}

	//This is a hack.
	//It's here because we're currently switching to rigidbody ground controllers, and
	//we want ground controllers to still work before we totally get rid of the
	//Character Controllers and switch over. This code should be completely gone by the
	//time we're using rigidbody ground controllers. Kill it with fire. Good luck. 
	if (FlightController.isFlying) {
		Mode = Modes.Flight;
		rigidbody.isKinematic = false;
	} else {
		Mode = Modes.Ground;
		rigidbody.isKinematic = true;
	}

	setMode ();
	}

	private bool disableGround() {
		bool ret = false;
		if (groundControllers.Count > 0) {
			foreach (MonoBehaviour gc in groundControllers) {
				gc.enabled = false;
			}	
			ret = true;
		}

		CharacterController cc = gameObject.GetComponent<CharacterController> ();
		if (cc) {
			cc.enabled = false;
		}
		return ret;
	}

	private bool enableGround() {
		if (groundControllers.Count > 0) {
			rigidbody.isKinematic = true;
			CharacterController cc = gameObject.GetComponent<CharacterController> ();
			if (cc) {
				cc.enabled = true;
			}
			foreach (MonoBehaviour gc in groundControllers) {
				gc.enabled = true;
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
			flightController.isFlying = false;
			return true;
		}
		return false;
	}

	private bool enableFlight() {
		if (FlightController) {
			rigidbody.isKinematic = false;
			rigidbody.freezeRotation = false;
			flightController.isFlying = true;
			return true;
		}
		Debug.LogError ("Failed to switch " + gameObject.name + " to flight controller. " +
		                "Make sure you have a valid flight controller attached. You can find" +
		                " Flight Controllers under FreeFlight/Scripts/Controllers.");
		return false;
	}

}
