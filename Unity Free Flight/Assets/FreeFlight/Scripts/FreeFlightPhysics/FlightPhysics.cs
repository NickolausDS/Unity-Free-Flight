using UnityEngine;
using UnityFreeFlight;
using System;
using System.Collections.Generic;


namespace UnityFreeFlight {

	/// <summary>
	/// Flight Physics determines the external forces acting on an airfoil-like
	/// object as it moves thought the air. It's responsible for calculating these
	/// forces, then applying them. 
	/// 
	/// There are three forces that Flight physics calculates: Lift, Drag, and a 
	/// non-physical force that corrects the objects directional velocity. 
	/// </summary>
	[Serializable]
	public class FlightPhysics {

		[Header ("Wing Properties")]

		//Common Raven
		//Span -- 42cm | Chord -- 125cm
		//Area -- .51 sqMet | Aspect Ratio -- 3:1 
		//Weight .69 - 2.0 KG
		//Turkey Vulture
		//Span -- 1.715M | Chord -- .7M
		//Area -- 1.2sqMet | Aspect Ratio -- 2.45:1
		//Weight .08 - 2.3 KG
		public float weight = 1f;	// in kilograms
		public float wingArea { get { return .51f; } }
		public float aspectRatio { get { return 3f; } }
		[Range(.7f, .999f)]
		public float wingEfficiency = 0.9f; 

		[HideInInspector]
		public float wingExposureArea;
		[Range (0.001f, 1.0f)]
		public float leftWingExposure;
		[Range (0.001f, 1.0f)]
		public float rightWingExposure;

		public Quaternion rotation;
		[NonSerialized]
		private List<Quaternion> physicsRotations = new List<Quaternion> ();
		[NonSerialized]
		private List<Mechanic> physicsRotationRegister = new List<Mechanic> ();

		private float _angleOfAttack;
		public float angleOfAttack { get { return _angleOfAttack; } }
		private float _airspeed;
		public float airspeed { get { return _airspeed; } }

		//Lift variables -- Forces are in Newtons/Second
		private float _liftCoefficient;
		public float liftCoefficient { get { return _liftCoefficient; } }
		private float _liftForce;
		public float liftForce { get { return _liftForce; } }
		private Vector3 _liftForceVector;
		public Vector3 liftForceVector { get { return _liftForceVector; } }

		//Drag Variables -- Forces are in Newtons/Second
		private float _dragCoefficient;
		public float dragCoefficient { get { return _dragCoefficient; } }
		private float _liftInducedDragForce;
		public float liftInducedDragForce { get { return _liftInducedDragForce; } }
		private float _formDragForce;
		public float formDragForce { get { return _formDragForce; } }
		private Vector3 _dragForceVector;
		public Vector3 dragForceVector { get { return _dragForceVector; } }

		public FlightPhysics () {
			//open the wings
			setWingExposure (1f, 1f);
		}

		/// <summary>
		/// Set the left and right wing exposure to the wind. zero is closed, and one
		/// is open. Therefore, .5 would be a half folded wing that would generate half
		/// as much lift. 
		/// </summary>
		/// <param name="cleftWingExposure">Cleft wing exposure.</param>
		/// <param name="crightWingExposure">Cright wing exposure.</param>
		public void setWingExposure(float cleftWingExposure, float crightWingExposure) {
			//Make sure area is never actually zero, as this is technically impossible and 
			//causes physics to fail.
			leftWingExposure = (cleftWingExposure == 0.0f) ? 0.01f : cleftWingExposure;
			rightWingExposure = (crightWingExposure == 0.0f) ? 0.01f : crightWingExposure;
			wingExposureArea = wingArea * (leftWingExposure + rightWingExposure) / 2;
		}

		/// <summary>
		/// Check if wings are open
		/// </summary>
		/// <returns><c>true</c>, if wings are open, <c>false</c> otherwise.</returns>
		public bool wingsOpen() {
			if (wingExposureArea == wingArea)
				return true;
			return false;
		}

		private int findRotationIndex(Mechanic id) {
			for (int i = 0; i < physicsRotationRegister.Count; i++) {
				if (physicsRotationRegister[i].Equals (id)) {
					return i;
				}
			}
			return -1;
		}

		public Quaternion getPhysicsRotation() {
			return rotation;
		}

		public void applyPhysicsRotation(Quaternion rotation, Mechanic registerID) {
			int id = findRotationIndex(registerID);
			if (id > -1) {
				physicsRotations[id] = rotation;
			} else {
				physicsRotations.Add (rotation);
				physicsRotationRegister.Add (registerID);
			}
		}

		public void releasePhysicsRotation(Mechanic id) {
			for (int i = 0; i < physicsRotationRegister.Count; i++) {
				if (physicsRotationRegister[i].Equals (id)) {
					physicsRotations.RemoveAt(i);
					physicsRotationRegister.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Calculate flight physics, then apply them to the rigidbody.
		/// </summary>
		/// <param name="rigidbody">Rigidbody.</param>
		public void applyPhysics(Rigidbody rigidbody) {

			rotation = rigidbody.rotation;
			foreach (Quaternion quat in physicsRotations) {
				rotation *= quat;
			}

			//TODO: swap rigidbody.velocity for the relative airspeed. Current calculation does not take into
			//account wind or other forces.
			physicsTick (rigidbody.velocity, rotation);

			if (rigidbody.isKinematic)
				return;
			
			rigidbody.rotation = getBankedTurnRotation(rigidbody.rotation);
			rigidbody.velocity = Vector3.Lerp (rigidbody.velocity, 
			                                   getDirectionalVelocity(rigidbody.rotation, rigidbody.velocity), 
			                                   Time.deltaTime);	

			rigidbody.AddForce (_liftForceVector);
			rigidbody.AddForce (_dragForceVector);
		}

		/// <summary>
		/// Calculate lift, drag, and angle of attack for this physics timestep.
		/// </summary>
		public void physicsTick(Vector3 relativeAirVelocity, Quaternion rotation) {
			if (relativeAirVelocity == Vector3.zero)
				return;

			_airspeed = relativeAirVelocity.magnitude;
			
			//These are required for computing lift and drag	
			_angleOfAttack = getAngleOfAttack(rotation, relativeAirVelocity);	
					
			//Calculate lift 
			_liftCoefficient = getLiftCoefficient(_angleOfAttack);
			_liftForce = getLift(_airspeed, wingExposureArea, _liftCoefficient);
			_liftForceVector = getLiftForceVector(relativeAirVelocity, _liftForce);

			//Calculate drag
			_dragCoefficient = getDragCoefficient (_angleOfAttack);
			_liftInducedDragForce = getLiftInducedDrag (_liftForce, _airspeed, wingExposureArea, wingEfficiency, aspectRatio);
			//TODO: wing exposure may not be a valid parameter to give here, because it assums wind is hitting the wings at a
			//ninety degree angle. This may be the reason for very high drag values. 
			_formDragForce = getFormDrag (_airspeed, wingExposureArea, _dragCoefficient);
			_dragForceVector = getDragForceVector (relativeAirVelocity, _liftInducedDragForce + _formDragForce);

		}

		/// <summary>
		/// Get a Vector 3 meausring the direction of the lift and the magnitude of the force.
		/// It's assumed that: unit=meter. 
		/// </summary>
		/// <returns>The lift 3d directional force vector.</returns>
		/// <param name="relativeAirVelocity">Relative air velocity.</param>
		/// <param name="lift">Amount of force the lift has generated (in newtons)</param>
		public Vector3 getLiftForceVector(Vector3 relativeAirVelocity, float lift) {
			if (relativeAirVelocity != Vector3.zero || lift == 0) {
				return Quaternion.LookRotation(relativeAirVelocity) * Vector3.up * liftForce;
			}
			return Vector3.zero;
		}
		
		
		/// <summary>
		/// 	Get the current lift force on the object in newtons
		/// </summary>
		/// <returns>The lift in Newtons</returns>
		/// <param name="velocity">Velocity Meters/Second</param>
		/// <param name="area">Area Meters^2</param>
		/// <param name="liftCoff">Lift coff. (dimensionless)</param>
		public float getLift(float velocity, float area, float liftCoff) {
			return .5f * velocity * velocity * WorldPhysics.pressure * area * liftCoff;
		}

		/// <summary>
		/// Get the lift coefficient at the specified angle of attack.
		/// </summary>
		/// <returns>The lift coefficient.</returns>
		/// <param name="angleDegrees">Angle degrees.</param>
		public float getLiftCoefficient(float angleDegrees) {
			angleDegrees = Mathf.Clamp (angleDegrees, -1f, 26.2f);
			return -1f / 100f * (angleDegrees * angleDegrees - 26f * angleDegrees - 31f);
		}

		/// <summary>
		/// Get 3D vector of the drag force
		/// </summary>
		/// <returns>The drag force vector.</returns>
		/// <param name="relativeAirVelocity">Relative air velocity.</param>
		/// <param name="drag">Drag force in newtnos</param>
		public Vector3 getDragForceVector(Vector3 relativeAirVelocity, float drag) {
			if (relativeAirVelocity != Vector3.zero)
				return Quaternion.LookRotation(relativeAirVelocity) * Vector3.back * drag;
			return Vector3.zero;
		}

		/// <summary>
		/// Get the drag caused by the shape and size of the airfoil. 
		/// </summary>
		/// <returns>The form drag (in newtons)</returns>
		/// <param name="velocity">airspeed</param>
		/// <param name="area">current wing exposure</param>
		/// <param name="dragCoff">Drag coff.</param>
		public float getFormDrag(float velocity, float area, float dragCoff) {
			return .5f * WorldPhysics.pressure * velocity * velocity * area * dragCoff;
		}

		/// <summary>
		/// Get drag created by generated lift
		/// </summary>
		/// <returns>The lift induced drag.</returns>
		/// <param name="lift">Lift in newtons per second</param>
		/// <param name="velocity">Velocity in meters/second.</param>
		/// <param name="area">current wing area exposure</param>
		/// <param name="wingefficiency">Efficiency of wing as an airfoil</param>
		/// <param name="aspectR">Aspect ratio of wing.</param>
		public float getLiftInducedDrag(float lift, float velocity, float area, float wingefficiency, float aspectR) {
			return (lift*lift) / (.5f * WorldPhysics.pressure * velocity * velocity * area * Mathf.PI * wingefficiency * aspectR);
		}

		/// <summary>
		/// Get the drag coefficient based on angle of attack and velocity. 
		/// </summary>
		/// <returns>The drag coefficient.</returns>
		/// <param name="angleDegrees">Angle degrees.</param>
		public float getDragCoefficient(float angleDegrees) {
			angleDegrees = Mathf.Clamp (angleDegrees, 0f, 22f);
			return .001f * angleDegrees * angleDegrees + .025f;
		}

		//When we do a turn, we don't just want to rotate our character. We want their
		//velocity to match the direction they are facing. 
		public Vector3 getDirectionalVelocity(Quaternion theCurrentRotation, Vector3 theCurrentVelocity) {
			Vector3 vel = theCurrentVelocity;
			
			vel = (theCurrentRotation * Vector3.forward).normalized * theCurrentVelocity.magnitude;	
			//Debug.Log (string.Format ("velocity: {0}, New Velocity {1} mag1: {2}, mag2 {3}", theCurrentVelocity, vel, theCurrentVelocity.magnitude, vel.magnitude));
			return vel;
		}
		
		/// <summary>
		/// Rotate the object around the Y axis based on how much roll it has. Imagine an airplane making a
		/// banked turn.
		/// </summary>
		/// <returns>The banked turn rotation.</returns>
		/// <param name="theCurrentRotation">The current rotation.</param>
		public Quaternion getBankedTurnRotation(Quaternion theCurrentRotation) {
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
		
		
		//Return angle of attack based on objects current directional Velocity and rotation
		public float getAngleOfAttack(Quaternion theCurrentRotation, Vector3 theCurrentVelocity) {
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
			//HACK: I'm not sure why, but sometimes this returns NAN. The reason should be found,
			//and fixed. For now, well just check if it's crazy and return sane things instead.
			return (float.IsNaN (theAngleOfAttack)) ? 0f : theAngleOfAttack;
		}
	}
}
