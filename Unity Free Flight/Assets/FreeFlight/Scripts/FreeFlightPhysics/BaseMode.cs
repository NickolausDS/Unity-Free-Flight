using UnityEngine;
using System;
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
	/// and braking for the third group. This class manages a single set of grouped actions
	/// called mechanics (a group like gliding, flapping, diving) Moreover, the mode manager 
	/// manages classes like this one, and decides which group of actions run.
	/// </summary>
	[Serializable]
	public class BaseMode {

		[HideInInspector]
		public string name = "Base Mode";

		public bool alwaysApplyPhysics;

		protected GameObject gameObject;
		protected Animator animator;
		protected Rigidbody rigidbody;
		[SerializeField][HideInInspector]
		protected SoundManager soundManager;

//		public List<string> mechanicNames = new List<string> ();
		protected List<Mechanic> mechanics = new List<Mechanic> ();
		protected Mechanic defaultMechanic;
		protected Mechanic currentMechanic = null;

		/// <summary>
		/// Since Unity's serialized scripts can't use complex constructors, we need to do
		/// initialization here instead. Init() sould be called after both the default constructor
		/// is called and deserialization takes place. It should be called *before* any methods
		/// are used, because most methods will depend on cached references set by init().
		/// </summary>
		/// <param name="go">Go.</param>
		public virtual void init (GameObject go, SoundManager sm) {
			gameObject = go;
			soundManager = sm;
			rigidbody = gameObject.GetComponent<Rigidbody> ();
			animator = gameObject.GetComponentInChildren<Animator> ();
		}

		public virtual void applyInputs () {
			
			applyMechanicPrecedence ();
			
			applyMechanic ();

			
			applyPhysics ();
		}

		/// <summary>
		/// Tells the manager to gather inputs from the player.
		/// This will happen in a frame-by-frame update time scale.
		/// </summary>
		public virtual void getInputs() {}

		/// <summary>
		/// Tweak anything that needs to happen before this modes mechanics run. 
		/// </summary>
		public virtual void startMode () {}

		/// <summary>
		/// Clean up any settings before this mode ends, including resetting things
		/// to be in a healthy state when this mode is run again. Please be nice to 
		/// the other modes, and reset any crazy values you temporarily set.
		/// </summary>
		public virtual void finishMode () {}
		
		protected virtual void applyPhysics() {
			alwaysApplyPhysics = false;
			throw new NotImplementedException("Physics for this mode has not been implemented. Disabling...");
		}

		
		/// <summary>
		/// The more serious version of applying physics. It calls the private method applyPhysics(), and
		/// has keyword 'only' to tell you ONLY FLIGHT PHYSICS WILL BE APPLIED. Just to be clear. 
		/// </summary>
		public void applyPhysicsOnly () {
			applyPhysics ();
		}
		
		/// <summary>
		/// Decide which mechanic should run. This solves players pressing multiple buttons at the
		/// same time.
		/// </summary>
		private void applyMechanicPrecedence() {
			foreach (Mechanic mech in mechanics) {
				if (mech.FFInputSatisfied () && isHigherPrecedence(mech)) {
					//If the current mechanic isn't done yet
					if (currentMechanic != null && !currentMechanic.FFFinish ())
						break;
					currentMechanic = mech;
					currentMechanic.FFBegin ();
					break;
				}
			}		
		}
		
		/// <summary>
		/// Apply the current mechanic behavior. 
		/// </summary>
		private void applyMechanic() {
			//Apply the current mechanic. 
			if (currentMechanic != null && currentMechanic != defaultMechanic) {
				
				currentMechanic.FFFixedUpdate ();
				
				if (!currentMechanic.FFInputSatisfied ()) {
					if (currentMechanic.FFFinish()) {
						currentMechanic = null;
					}
				}
			} else {
				defaultMechanic.FFFixedUpdate ();
			}
		}
		
		private bool isHigherPrecedence(Mechanic mech) {
			if (currentMechanic == null)
				return true;
			
			int currentMechIndex = -1;
			int otherMechIndex = -1;
			for (int i = 0; i < mechanics.Count; i++) {
				if (currentMechanic == mechanics[i])
					currentMechIndex = i;
				if (mech == mechanics[i])
					otherMechIndex = i;
			}
			return (otherMechIndex < currentMechIndex ? true : false);
		}


	}

}
