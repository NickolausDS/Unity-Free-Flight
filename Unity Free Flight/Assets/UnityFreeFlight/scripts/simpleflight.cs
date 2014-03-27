using UnityEngine;
using System.Collections;

public class simpleflight : MonoBehaviour {
	
	//GUI buttons
	public bool toggleStatsMenu = true;
	public bool togglePhysicsMenu = true;
	public bool toggleGravity = true;
	public bool toggleLift = false;
	public bool toggleDrag = true;

		
		
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
		
		
	//FLYING BODY SPECIFICATIONS
	public float wingChord; //in meters
	public float wingSpan;  //in meters
	public float weight;	// in kilograms
	//generated vars
	public float wingArea; // span * chord
	public float aspectRatio; //span / chord
	public float liftToWeightRatio; // will be important, not using it now.
	//End flying body statistics
		
		
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
		//make wing dimensions
		iAmATurkeyVulture();
		wingArea = wingSpan * wingChord;
		aspectRatio = wingSpan / wingChord;
			
			
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
		newRotation = getBankedTurnRotation(newRotation);
		//Correct our velocity for the new direction we are facing
		//		newVelocity = getDirectionalVelocity(newRotation, newVelocity);	
		newVelocity = Vector3.Lerp (newVelocity, getDirectionalVelocity(newRotation, newVelocity), Time.deltaTime);	

			
		//These are required for computing lift and drag	
		angleOfAttack = getAngleOfAttack(newRotation, newVelocity);	
		liftCoefficient = getLiftCoefficient(angleOfAttack);

		if (newVelocity != Vector3.zero) {

			// apply lift force
			liftForce = getLift(newVelocity.magnitude, 0, wingArea, liftCoefficient) * Time.deltaTime;
			Vector3 directionalLift = Quaternion.LookRotation(newVelocity) * Vector3.up;
			vlift =  (directionalLift * liftForce);
			if (toggleLift) {
				rigidbody.AddForce(vlift, ForceMode.Force);
			}
			
			// get drag rotation
			dragForce = getDrag(liftForce, 0, newVelocity.magnitude, wingArea, aspectRatio) * Time.deltaTime;
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
		
	//Get new yaw and roll, store the value in newRotation
	Quaternion getBankedTurnRotation(Quaternion theCurrentRotation) {
	//Quaternion getBankedTurnRotation(float curZRot, float curLift, float curVel, float mass) {
		// The physics of a banked turn is as follows
		//  L * Sin(0) = M * V^2 / r
		//	L is the lift acting on the aircraft
		//	θ0 is the angle of bank of the aircraft
		//	m is the mass of the aircraft
		//	v is the true airspeed of the aircraft
		//	r is the radius of the turn	
		//
		// Currently, we'll keep turn rotation simple. The following is not based on the above, but it provides
		// A pretty snappy mechanism for getting the job done.
		//Apply Yaw rotations. Yaw rotation is only applied if we have angular roll. (roll is applied directly by the 
		//player)
		Quaternion angVel = Quaternion.identity;
		//Get the current amount of Roll, it will determine how much yaw we apply.
		float zRot = Mathf.Sin (theCurrentRotation.eulerAngles.z * Mathf.Deg2Rad) * Mathf.Rad2Deg;
		//We don't want to change the pitch in turns, so we'll preserve this value.
		float prevX = theCurrentRotation.eulerAngles.x;
		//Calculate the new rotation. The constants determine how fast we will turn.
		Vector3 rot = new Vector3(0, -zRot * 0.8f, -zRot * 0.5f) * Time.deltaTime;
			
		//Apply the new rotation 
		angVel.eulerAngles = rot;
		angVel *= theCurrentRotation;	
		angVel.eulerAngles = new Vector3(prevX, angVel.eulerAngles.y, angVel.eulerAngles.z);
			
		//Done!
		return angVel;	
	}
		
	//When we do a turn, we don't just want to rotate our character. We want their
	//velocity to match the direction they are facing. 
	Vector3 getDirectionalVelocity(Quaternion theCurrentRotation, Vector3 theCurrentVelocity) {
		Vector3 vel = theCurrentVelocity;
			
		vel = (theCurrentRotation * Vector3.forward).normalized * theCurrentVelocity.magnitude;	
		//Debug.Log (string.Format ("velocity: {0}, New Velocity {1} mag1: {2}, mag2 {3}", theCurrentVelocity, vel, theCurrentVelocity.magnitude, vel.magnitude));
		return vel;
	}
	//Return angle of attack based on objects current directional Velocity and rotation
	float getAngleOfAttack(Quaternion theCurrentRotation, Vector3 theCurrentVelocity) {
		//Angle of attack is basically the angle air strikes a wing. Imagine a plane flying 
		//at exact level altitude into a stable air mass. The air passes over the wing very
		//efficiently, so we have an AOA of zero. When the plane pitches back, air starts to
		//strike the bottom of the wing, creating more drag and lift. The angle of pitch 
		//relative to the airmass is called angle of attack. 
		float theAngleOfAttack;
		//The direction we are going
		Vector3 dirVel;

		//We need speed in order to get directional velocity.
		if (theCurrentVelocity != Vector3.zero) {
			//Find the direction we are going
			dirVel = Quaternion.LookRotation(theCurrentVelocity) * Vector3.up;
		} else {
			//This has the effect of 'imagining' the craft is on a level flight
			//moving forward. Since angle of attack means nothing at zero speed,
			//this is simply a way to visualize it when we are dead stopped.
			dirVel = Vector3.up;
		}

		//		Debug.Log(string.Format ("Directional Velocity : {0}", dirVel));

		//Find the rotation directly in front of us
		Vector3 forward = theCurrentRotation * Vector3.forward;
		//The dot product returns a positive or negative float if we are 'pitched up' towards
		//our air mass, or 'pitched down into' our airmass. Remember that our airmass also has
		//a velocity coming towards us, which is somewhere between coming directly at us in
		//level flight, or if we are falling directly towards the ground, it is coming directly
		//below us. 

		//The dot product always returns between -1 to 1, so taking the ArcSin will give us
		//a reasonable angle of attack. Remember to convert to degrees from Radians. 
		theAngleOfAttack = Mathf.Asin(Vector3.Dot(forward, dirVel)) * Mathf.Rad2Deg;
		return theAngleOfAttack;
	}

	/*
		 Velocity -- must be in meters/second
		 pressure -- don't worry about this for now
		 area	  -- something good
		 angleOfAttack -- given in forward pitch relative to speed
		 
		 returns: 
		 
	*/ 
	float getLift(float velocity, float pressure, float area, float liftCoff) {
			//pressure = .45817f;
			pressure = 1.225f;
			//attackAngle = 10.0f;
			
			float lift = velocity * velocity * pressure * area * liftCoff;
			return lift;
	}
		
		
	float getDrag(float lift, float pressure, float velocity, float area, float aspectR) {
			//wing span efficiency value
			float VSEV = .9f;
			pressure = 1.225f;

			LiftInducedDrag = (lift*lift) / (.5f * pressure * velocity * velocity * area * Mathf.PI * VSEV * aspectR);
			formDrag = .5f * pressure * velocity * velocity * getDragCoefficient(angleOfAttack) * area;
			return LiftInducedDrag + formDrag;
	}
		
	float getLiftCoefficient(float angleDegrees) {
			float cof;
//			if(angleDegrees > 40.0f)
//				cof = 0.0f;
//			if(angleDegrees < 0.0f)
//				cof = angleDegrees/90.0f + 1.0f;
//			else
//				cof = -0.0024f * angleDegrees * angleDegrees + angleDegrees * 0.0816f + 1.0064f;
		//Formula based on theoretical thin airfoil theory. We get a very rough estimate here,
		//and this does not take into account wing aspect ratio
		cof = 2 * Mathf.PI * angleDegrees * Mathf.Deg2Rad;
			return cof;

		}
		
	float getDragCoefficient(float angleDegrees) {
			float cof;
			//if(angleDegrees < -20.0f)
			//	cof = 0.0f;
			//else
			cof = .0039f * angleDegrees * angleDegrees + .025f;
			return cof;
		}
		
		
	void iAmATurkeyVulture() 	{
		wingSpan = 1.715f;
		wingChord = .7f;
		weight = 1.55f;
		
	}
		
	//testing purposes	
	void OnGUI() {
			
		toggleStatsMenu = GUILayout.Toggle(toggleStatsMenu, "Show Stats");
		togglePhysicsMenu = GUILayout.Toggle(togglePhysicsMenu, "Show Physics");
		
			
		if (toggleStatsMenu) {
			GUI.Box(new Rect(310, 10, 400, 120), string.Format ("Stats:\nWing Span: {0} M\n Wing Chord: {1} M\n Total Wing Area: {2} M^2\nAspect Ratio: {3} S/C\n Weight: {4} Newtons\n Lift-to-Weight ratio: {5}",
					wingSpan,
					wingChord,
					wingArea,
					aspectRatio,
					weight,
					liftToWeightRatio
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
					LiftInducedDrag,
					formDrag,
					rigidbody.drag,
					angleOfAttack, 
					liftCoefficient)
				);	
			toggleLift = GUILayout.Toggle(toggleLift, "Lift Force");
			toggleDrag = GUILayout.Toggle(toggleDrag, "Drag Force");
			toggleGravity = GUILayout.Toggle(toggleGravity, "Gravity");
		}
				
	}

	float sinDeg(float degrees) {
		return Mathf.Sin (degrees * Mathf.Deg2Rad) * Mathf.Rad2Deg;
	}
		
	float cosDeg(float degrees) {
		return Mathf.Cos (degrees * Mathf.Deg2Rad) * Mathf.Rad2Deg;

	}
	
	
}
