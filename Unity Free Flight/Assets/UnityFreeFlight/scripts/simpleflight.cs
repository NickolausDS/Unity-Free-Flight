using UnityEngine;
using System.Collections;

public class simpleflight : MonoBehaviour {
	
	//GUI buttons
	public bool toggleStatsMenu = true;
	public bool togglePhysicsMenu = true;
	public bool toggleGravity = true;
	public bool toggleLift = false;
	public bool toggleDrag = true;

	private FlightPhysics fPhysics = new FlightPhysics ();
	//We initialize this at start()
	private FlightBody fBody = null;
		
	//angle at which flying body contacts an air mass
	//(A plane/bird has a high angle of attack when they land, nose up, into the wind)
	public float angleOfAttack;
	public float liftCoefficient = 1.0f;
	//Constant speed with which we'll rotate. Doesn't have effect on physics.
	public float RotationSpeed = 200.0f;
		
		
		
	//FORCES 	
	//The computed force of lift our flying body generates.
	public float liftForce;
	//The speed of our flying body
	public float AmbientSpeed = 20.0f;
	//The drag againsnt our flying body
	public float dragForce;
	//Gravity currently set by Physics.gravity
	//public float gravity = 0.2f;
	public float LiftInducedDrag;
	public float formDrag;
		
		
//	//FLYING BODY SPECIFICATIONS
//	[SerializeField]
//	private float wingChord; 
//	public float WingChord{ get{return wingChord;} set{wingChord = value;}} //in meters
//	[SerializeField]
//	private float wingSpan;
//	public float WingSpan { get{return wingSpan;} set{wingSpan = value;}}  //in meters
//	public float weight;	// in kilograms
//	//generated vars
//	public float wingArea { get{return fBody.WingArea;} set{fBody.WingArea = value;}} // span * chord
//	public float aspectRatio; //span / chord
//	public float liftToWeightRatio; // will be important, not using it now.
//	//	End flying body statistics
		
		
	public Vector3 TESTVELOCITY;
	public float TESTMAGNITUDE;
	public Vector3 moveRotation;
	public Vector3 anglularVelocity;
	public Vector3 moveDirection = Vector3.forward;
	public Vector3 vdrag = Vector3.back;
	public Vector3 vlift = Vector3.up;

	public Vector3 userRotationInput;
	public Quaternion curRotation;
	public Vector3 curVelocity;
	public Quaternion newRotation;
	public Vector3 newVelocity;
		
	void Start() {

		fBody = gameObject.GetComponent<FlightBody> ();
		Debug.Log (string.Format ("{0}", fBody));
		if (fBody == null)
			fBody = gameObject.AddComponent<FlightBody> ();
		//make wing dimensions
//		iAmATurkeyVulture();
//		wingArea = wingSpan * wingChord;
//		aspectRatio = wingSpan / wingChord;
//		wingArea = fBody.WingArea;
//		aspectRatio = fBody.AspectRatio;

			
			
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
		float dragCoefficient = fPhysics.getDragCoefficient (angleOfAttack);

		if (newVelocity != Vector3.zero) {

			// apply lift force
			liftForce = fPhysics.getLift(newVelocity.magnitude, 0, fBody.WingArea, liftCoefficient) * Time.deltaTime;
			Vector3 directionalLift = Quaternion.LookRotation(newVelocity) * Vector3.up;
			vlift =  (directionalLift * liftForce);
			if (toggleLift) {
				rigidbody.AddForce(vlift, ForceMode.Force);
			}
			
			// get drag rotation
			dragForce = fPhysics.getDrag(newVelocity.magnitude,0, fBody.WingArea, dragCoefficient, liftForce, fBody.AspectRatio) * Time.deltaTime;
			Vector3 directionalDrag = Quaternion.LookRotation(newVelocity) * Vector3.back;
			// Debug.Log(string.Format ("Drag Direction: {0}, Drag Newtons/Hour: {1}", directionalDrag, dragForce * 3600.0f));
			vdrag = (directionalDrag * dragForce);
			if (toggleDrag) {
				rigidbody.AddForce (vdrag);
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
		

	/*
		 Velocity -- must be in meters/second
		 pressure -- don't worry about this for now
		 area	  -- something good
		 angleOfAttack -- given in forward pitch relative to speed
		 
		 returns: 
		 
	*/ 
	
		
		
	//testing purposes	
	void OnGUI() {
			
		toggleStatsMenu = GUILayout.Toggle(toggleStatsMenu, "Show Stats");
		togglePhysicsMenu = GUILayout.Toggle(togglePhysicsMenu, "Show Physics");
		
			
		if (toggleStatsMenu) {
			GUI.Box(new Rect(310, 10, 400, 120), string.Format ("Stats:\nWing Span: {0} M\n Wing Chord: {1} M\n Total Wing Area: {2} M^2\nAspect Ratio: {3} S/C\n Weight: {4} Newtons\n",
					fBody.WingSpan,
					fBody.WingChord,
					fBody.WingArea,
					fBody.AspectRatio,
					fBody.Weight,
				));		
				
		}
		
		if (togglePhysicsMenu) {
			GUI.Box(new Rect(100,10,200,200), string.Format("Physics:\nspeed Vector: {0}\nSpeed: {1} Km/h\nDirection {2}\nGravity: {3}\nAltitude+-: {4}\nLift N/H: {5}\nDrag N/H: {6}\n\tInduced{7}\n\tForm {8}\n RigidBody Drag: {9} \nAngle Of Attack: {10}\nLift COF: {11}", 
					rigidbody.velocity,
					rigidbody.velocity.magnitude * 3600.0f / 1000.0f,
					rigidbody.rotation.eulerAngles,
					Physics.gravity.y, 
					liftForce + Physics.gravity.y * 3600.0f / 1000.0f, 
					liftForce * 3600.0f / 1000.0f,
					dragForce * 3600.0f / 1000.0f,
					fPhysics.LiftInducedDrag,
					fPhysics.FormDrag,
					rigidbody.drag,
					angleOfAttack, 
					liftCoefficient)
				);	
			toggleLift = GUILayout.Toggle(toggleLift, "Lift Force");
			toggleDrag = GUILayout.Toggle(toggleDrag, "Drag Force");
			toggleGravity = GUILayout.Toggle(toggleGravity, "Gravity");
		}
				
	}

	
}
