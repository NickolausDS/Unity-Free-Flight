using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityFreeFlight;



namespace UnityFreeFlight {

	/// <summary>
	/// A base mode defines a type of movement that will be switched between with a 
	/// Mode Manager. The movement type includes a set of grouped actions such as 
	/// flying, walking, driving. These grouped actions are meant to be abstractions
	/// for further 'mechanics' such as gliding, flapping, diving for the first group, 
	/// standing, walking, running, jumping for the second group, and turning, accelerating, 
	/// and braking for the third group. 
	/// </summary>
	public abstract class BaseMode {

		public bool alwaysApplyPhysics;

		protected GameObject gameObject;
		protected Animator animator;
		protected Rigidbody rigidbody;

		public BaseMode (GameObject go) {
			gameObject = go;
			rigidbody = gameObject.GetComponent<Rigidbody> ();
			animator = gameObject.GetComponentInChildren<Animator> ();


		}
		/// <summary>
		/// Needs access to physics
		/// Will have a list of Mechanics classes, like flapping
		/// 
		/// </summary>

//		public List<Mechanic> flightMechanics; 
//
////		private int defaultAnimationState;
////		private int currentAnimationState;
//		//The mechanic we fall back on if no mechanics are in effect
//		private Mechanic _defaultMechanic;
//		public Mechanic defaultMechanic { get; set;}
//		//The current mechanic in effect;
//		private Mechanic currentMechanic;
//
//		/// <summary>
//		/// Fires off a mechanic based on what inputs are being set by the player. Mechanics 
//		/// follow a strict priority set in the editor, and may exclude one another from happening.
//		/// For example, the player can hold down both flap and dive inputs, but flapping takes
//		/// precedence (if flapping is set higher in the list). Mechanics can contain their own set of
//		/// physics (such as moving the player upwards in a flap) and also drive setting all variables
//		/// in the Animation Controller.
//		/// </summary>
//		public void applyInputs() {
//			//The next mechanic we might use
//			Mechanic nextMechanic = null;
//
//			foreach (Mechanic mech in flightMechanics) {
//				if (mech.enabled && mech.FFInputSatisfied ()) {
//					nextMechanic = mech;
//					mech.FFStart (animator, rigidbody);
//				} else if (mech == currentMechanic && !mech.FFInputSatisfied()) {
//					mech.FFFinish(animator, rigidbody);
//				}
//			}
//
//			if (nextMechanic != null) 
//				currentMechanic = nextMechanic;
//
//			currentMechanic.FFFixedUpdate (animator, rigidbody);
//			applyPhysics ();
//		}
//

		/// <summary>
		/// Tells the manager to gather inputs from the player.
		/// This will happen in a frame-by-frame update time scale.
		/// </summary>
		public abstract void getInputs();

		public abstract void applyInputs();

		public abstract void startMode ();

		public abstract void finishMode ();

		protected abstract void applyPhysics();

		/// <summary>
		/// The more serious version of applying physics. It calls the private method applyPhysics(), and
		/// has keyword 'only' to tell you ONLY FLIGHT PHYSICS WILL BE APPLIED. Just to be clear. 
		/// </summary>
		public void applyPhysicsOnly () {
			applyPhysics ();
		}


	}

}
