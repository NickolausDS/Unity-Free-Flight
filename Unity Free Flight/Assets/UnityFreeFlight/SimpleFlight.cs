using UnityEngine;
using System.Collections;


public class SimpleFlight : MonoBehaviour {

	//Note: 4/30/14 -- These, along with the GUI view, should be moved to their own separate
	//class. They no longer belong here.
	//GUI buttons
	public bool toggleStatsMenu = false;
	public bool togglePhysicsMenu = false;
	public bool toggleWorldPhysicsMenu = false;
	public bool toggleGravity = true;
	public bool toggleLift = true;
	public bool toggleDrag = true;

	private FlightPhysics fPhysics = new FlightPhysics ();
	//We initialize this at start()
	public FlightObject fObj = new FlightObject ();

	public BaseFlightController flightController = null;
	public MonoBehaviour groundController = null;
	public bool _groundMode = false;
		


	//NOTE: 4/23/14 -- These need to be refactored and moved to the physics objects. They don't belong here.
	//PHYSICS VARS
	//These are defined here for efficiency, so they aren't re-initialized every time
	//we run FixedUpdate(). They all get new derived values every update run. You probably
	//shouldn't edit them outside of FixedUpdate()
	private float liftForce;
	private float dragForce;
	private Vector3 directionalLift;
	private Vector3 directionalDrag;
	private float liftCoefficient;
	private float dragCoefficient;
	//angle at which flying body contacts an air mass
	//(A plane/bird has a high angle of attack when they land, nose up, into the wind)
	private float angleOfAttack;
	private Quaternion newRotation;
	private Vector3 newVelocity;
		
	void Start() {
		rigidbody.velocity = new Vector3(0.0f, 0.0f, 20.0f);
		// We don't want the rigidbody to determine our rotation,
		// we will compute that ourselves
		rigidbody.freezeRotation = true;
		//This behaviour will be done by the editor instead
//		controller = gameObject.GetComponent<BaseFlightController>();
//		groundController = (MonoBehaviour) gameObject.AddComponent<BaseGroundController>();
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
		
	//NOTE 4/23/14: Most of this needs to be refactored and moved to the physics objects.
	//None of it should really exist here.
	void FixedUpdate() {

		if (!_groundMode) {
			rigidbody.useGravity = toggleGravity;
				
			//These will be used to compute new values	
			newRotation = rigidbody.rotation;	
			newVelocity = rigidbody.velocity;
			
			//Find out how much our user turned us
			newRotation *= flightController.UserInput;
			//Apply the user rotation in a banked turn
			newRotation = fPhysics.getBankedTurnRotation(newRotation);
			//Correct our velocity for the new direction we are facing
			//		newVelocity = getDirectionalVelocity(newRotation, newVelocity);	
			newVelocity = Vector3.Lerp (newVelocity, fPhysics.getDirectionalVelocity(newRotation, newVelocity), Time.deltaTime);	

				
			//These are required for computing lift and drag	
			angleOfAttack = fPhysics.getAngleOfAttack(newRotation, newVelocity);	
			liftCoefficient = fPhysics.getLiftCoefficient(angleOfAttack);
			dragCoefficient = fPhysics.getDragCoefficient (angleOfAttack);

			if (newVelocity != Vector3.zero) {

				// apply lift force
				liftForce = fPhysics.getLift(newVelocity.magnitude, 0, fObj.WingArea, liftCoefficient) * Time.deltaTime;
				directionalLift = Quaternion.LookRotation(newVelocity) * Vector3.up;
				if (toggleLift) {
					rigidbody.AddForce(directionalLift * liftForce);
				}
				
				// get drag rotation
				dragForce = fPhysics.getDrag(newVelocity.magnitude,0, fObj.WingArea, dragCoefficient, liftForce, fObj.AspectRatio) * Time.deltaTime;
				directionalDrag = Quaternion.LookRotation(newVelocity) * Vector3.back;
				// Debug.Log(string.Format ("Drag Direction: {0}, Drag Newtons/Hour: {1}", directionalDrag, dragForce * 3600.0f));
				if (toggleDrag) {
					rigidbody.AddForce (directionalDrag * dragForce);
				}
			
			}
			//Finally, apply all the physics on our actual rigidbody
			rigidbody.rotation = newRotation;
			rigidbody.velocity = newVelocity;
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
			_groundMode = !_groundMode;
		}

	}

	private void switchToFlight() {
		if (flightController) {
			flightController.flightEnabled = true;
			if (groundController) {
				groundController.enabled = false;
				CharacterController cc = gameObject.GetComponent<CharacterController> ();
				if (cc) {
					cc.enabled = false;
				}
			}
		} else {
			Debug.Log ("No air controller detected, not switching to air controls");
			_groundMode = !_groundMode;
		}
	}
	

	//We might consider moving this outside this class, to keep things simpler
	void OnGUI() {
			
		toggleStatsMenu = GUILayout.Toggle(toggleStatsMenu, "Show Stats");
		togglePhysicsMenu = GUILayout.Toggle(togglePhysicsMenu, "Show Physics");
		
			
		if (toggleStatsMenu) {
			GUI.Box(new Rect(310, 10, 200, 110), 
			        string.Format ("Stats:\nWing Span: {0:###.#}{1}\n " +
			               "Wing Chord: {2:###.#}{3}\n " +
			               "Total Wing Area: {4:###.#}{5}\n" +
			               "Aspect Ratio: {6:#.#}\n " +
			               "Weight: {7:###.#}{8}\n",
							fObj.WingSpan, fObj.getLengthType(),
			        		fObj.WingChord, fObj.getLengthType(),
			               	fObj.WingArea, fObj.getAreaType(),
							fObj.AspectRatio,
			               	fObj.Weight, fObj.getWeightType()
				));		
				
		}
		
		if (togglePhysicsMenu) {
			GUI.Box(new Rect(100,10,200,140), string.Format(
				"Physics:\n" +
				"Speed: {0:###.#}{1}\n" +
				"Altitude+-: {2:###.#}{3}\n" +
				"Lift N/H: {4:###.#}{5}\n" +
				"Drag N/H: {6:###.#}{7}\n" +
				"\tInduced: {8:###.#}{9}\n" +
				"\tForm {10:###.#}{11}\n " +
				"Angle Of Attack: {12:##}{13}\n" +
				"Lift COF: {14:#.##}", 
					rigidbody.velocity.magnitude * 3600.0f / 1000.0f, "KPH",
					liftForce + Physics.gravity.y * 3600.0f / 1000.0f, "M",
					liftForce * 3600.0f / 1000.0f, "N",
					dragForce * 3600.0f / 1000.0f, "N",
					fPhysics.LiftInducedDrag, "N",
					fPhysics.FormDrag, "N",
					angleOfAttack, "Deg",
					liftCoefficient)
				);
			if (toggleWorldPhysicsMenu) {
				GUI.Box (new Rect(100, 160, 200, 90), string.Format (
					"World Physics:\n" +
					"speed Vector: {0}\n" +
					"Direction {1}\n" +
					"Gravity: {2}\n" +
					"RigidBody Drag: {3} \n",
					rigidbody.velocity,
					rigidbody.rotation.eulerAngles,
					Physics.gravity.y, 
					rigidbody.drag
					));
			}

			toggleLift = GUILayout.Toggle(toggleLift, "Lift Force");
			toggleDrag = GUILayout.Toggle(toggleDrag, "Drag Force");
			toggleGravity = GUILayout.Toggle(toggleGravity, "Gravity");
			toggleWorldPhysicsMenu = GUILayout.Toggle(toggleWorldPhysicsMenu, "World Physics");
		}
				
	}

	
}
