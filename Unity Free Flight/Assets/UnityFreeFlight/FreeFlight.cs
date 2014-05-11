using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class FreeFlight : MonoBehaviour {

	
	public BaseFlightController flightController = null;
	public MonoBehaviour groundController = null;
	private bool _groundMode = false;
	private FlightPhysics _physicsObject;

		
	void Start() {
		rigidbody.velocity = new Vector3(0.0f, 0.0f, 20.0f);
		// We don't want the rigidbody to determine our rotation,
		// we will compute that ourselves
		rigidbody.freezeRotation = true;
		//This behaviour will be done by the editor instead
//		controller = gameObject.GetComponent<BaseFlightController>();
//		groundController = (MonoBehaviour) gameObject.AddComponent<BaseGroundController>();
	}
		
	public FlightPhysics PhysicsObject { 
		get {
			if (_physicsObject == null)
				_physicsObject = new FlightPhysics (rigidbody);
			return _physicsObject; 
			} 
		}

	public bool GroundMode {
		get {return _groundMode;}
		set {
			if(_groundMode == value)
				return;
			_groundMode = value;
			if (_groundMode)
				switchToGround();
			else
				switchToFlight();
		}
	}
		
	void FixedUpdate() {

		if (!_groundMode) {
			PhysicsObject.doStandardPhysics(flightController.UserInput);
		}
	}

	private void switchToGround() {

		if (groundController) {
			groundController.enabled = true;
			flightController.flightEnabled = false;
			CharacterController cc = gameObject.GetComponent<CharacterController> ();
			if (cc) {
				cc.enabled = true;
			}
		} else {
			Debug.Log ("No ground controller detected, not switching to ground controls");
			GroundMode = !GroundMode;
		}

	}

	private void switchToFlight() {
		if (flightController) {
			flightController.flightEnabled = true;
			if (groundController) {
				groundController.enabled = false;
			}
			CharacterController cc = gameObject.GetComponent<CharacterController> ();
			if (cc) {
				cc.enabled = false;
			}
		} else {
			Debug.Log ("No air controller detected, not switching to air controls");
			GroundMode = !GroundMode;
		}
	}


	
}
