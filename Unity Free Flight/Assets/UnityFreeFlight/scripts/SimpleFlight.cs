using UnityEngine;
using System.Collections;

public class SimpleFlight : MonoBehaviour {
	
	//GUI buttons
	public bool toggleStatsMenu = true;
	public bool togglePhysicsMenu = true;
	public bool toggleWorldPhysicsMenu = true;
	public bool toggleGravity = true;
	public bool toggleLift = false;
	public bool toggleDrag = true;

	private FlightPhysics fPhysics = new FlightPhysics ();
	//We initialize this at start()
	public FlightBody fBody = new FlightBody ();
		

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
	

	private Vector3 userRotationInput;
	//Constant speed with which we'll rotate with user controls
	public float RotationSpeed = 200.0f;
		
	void Start() {

//		fBody = gameObject.GetComponent<FlightBody> ();
//		Debug.Log (string.Format ("{0}", fBody));
//		if (fBody == null)
//			fBody = gameObject.AddComponent<FlightBody> ();
			
		rigidbody.velocity = new Vector3(0.0f, 0.0f, 20.0f);
		// We don't want the rigidbody to determine our rotation,
		// we will compute that ourselves
		rigidbody.freezeRotation = true;
	}
		
	void Update() {
		
		//Pitch
		userRotationInput.x = -Input.GetAxis("Vertical") * (RotationSpeed * Time.deltaTime);
	    //Roll
		userRotationInput.z = -Input.GetAxis("Horizontal") * (RotationSpeed * Time.deltaTime);
	    //Yaw
	//	userRotationInput.y = Input.GetAxis("Yaw") * (RotationSpeed * Time.deltaTime);	
	}
		
	void FixedUpdate() {
		rigidbody.useGravity = toggleGravity;
			
		//These will be used to compute new values	
		newRotation = rigidbody.rotation;	
		newVelocity = rigidbody.velocity;
		
		//Find out how much our user turned us
		newRotation *= getUserRotation(userRotationInput);
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
			liftForce = fPhysics.getLift(newVelocity.magnitude, 0, fBody.WingArea, liftCoefficient) * Time.deltaTime;
			directionalLift = Quaternion.LookRotation(newVelocity) * Vector3.up;
			if (toggleLift) {
				rigidbody.AddForce(directionalLift * liftForce);
			}
			
			// get drag rotation
			dragForce = fPhysics.getDrag(newVelocity.magnitude,0, fBody.WingArea, dragCoefficient, liftForce, fBody.AspectRatio) * Time.deltaTime;
			directionalDrag = Quaternion.LookRotation(newVelocity) * Vector3.back;
			// Debug.Log(string.Format ("Drag Direction: {0}, Drag Newtons/Hour: {1}", directionalDrag, dragForce * 3600.0f));
			if (toggleDrag) {
				rigidbody.AddForce (directionalDrag * dragForce);
			}
		
		}
		//Finally, apply all the physics on our actual rigidbody
		rigidbody.rotation = newRotation;
		rigidbody.velocity = newVelocity;	
		
		//MAX FORCE CONSTRAINT
	//	if(rigidbody.velocity.magnitude > 100) {
	//			Debug.Log(string.Format("----- MAX FORCE CONSTRAINT WARNING -----\n\nTime {0}\nVelocity: {1} \nRotation {2} \nMagnitude {3}\n\n-----  -----",
	//				Time.realtimeSinceStartup, rigidbody.velocity, rigidbody.rotation.eulerAngles, rigidbody.velocity.magnitude));
	//			rigidbody.velocity *= 0.9f;
	//
	//		}
	}

	Quaternion getUserRotation(Vector3 theUserRotationInput) {
		Quaternion theNewRotation = Quaternion.identity;
		theNewRotation.eulerAngles = theUserRotationInput;
		return theNewRotation;
	}

	//testing purposes	
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
							fBody.WingSpan, fBody.getLengthType(),
			        		fBody.WingChord, fBody.getLengthType(),
			               	fBody.WingArea, fBody.getAreaType(),
							fBody.AspectRatio,
			               	fBody.Weight, fBody.getWeightType()
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
