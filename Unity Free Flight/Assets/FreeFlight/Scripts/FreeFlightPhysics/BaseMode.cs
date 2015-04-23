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



//		public List<string> mechanicNames = new List<string> ();
		[NonSerialized]
		public List<Mechanic> mechanics = new List<Mechanic> ();
		[NonSerialized]
		public Mechanic defaultMechanic;
		[NonSerialized]
		public Mechanic currentMechanic = null;
		[NonSerialized]
		public Mechanic finishMechanic;
		[NonSerialized]
		public LinkedList<Mechanic> currentMechanics = new LinkedList<Mechanic> ();


		public List<string> mechanicTypeNames = new List<string> ();
		public string defaultMechanicTypeName = "";
		public string finishMechanicTypeName;



		/// <summary>
		/// Since Unity's serialized scripts can't use complex constructors, we need to do
		/// initialization here instead. Init() sould be called after both the default constructor
		/// is called and deserialization takes place. It should be called *before* any methods
		/// are used, because most methods will depend on cached references set by init().
		/// </summary>
		/// <param name="go">Go.</param>
		public virtual void init (GameObject go) {
			gameObject = go;
			rigidbody = gameObject.GetComponent<Rigidbody> ();
			animator = gameObject.GetComponentInChildren<Animator> ();
//			setupMechanics ();
		}

//		public virtual void setupMechanics() {
//			throw new NotImplementedException ("setupMechanics needs to be implemented");
//		}

		public virtual void applyInputs () {

			try {
//				applyMechanicPrecedence ();
//				simplePrecedenceChaining ();
				recursivePrecedenceChaining ();

//				applyMechanic ();
				applyMechanics ();
			} catch (Exception e) {
				Debug.LogException(e);
			}

			
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
				if (mech.enabled && mech.FFInputSatisfied () && isHigherPrecedence(mech)) {
					//If the current mechanic isn't done yet
					if (currentMechanic != null && !currentMechanic.FFFinish ())
						break;
					currentMechanic = mech;
					currentMechanic.FFStart ();
					break;
				}
			}		
		}

		/// <summary>
		/// Does NOT check what the current mechanic chains to. Instead, all other mechanics
		/// with less precedence are automatically chained. 
		/// </summary>
		private void simplePrecedenceChaining() {
			if (currentMechanics.First == null)
				return;

			for (int i = getPrecedence (currentMechanics.First.Value); i < mechanics.Count; i++) {
				if (mechanics[i].FFInputSatisfied() && !currentMechanics.Contains(mechanics[i])) {
					currentMechanics.AddLast(mechanics[i]);
					mechanics[i].FFStart();
				}
			}
		}

		/// <summary>
		/// The highest precendence mechanic runs first. If the mechanic contains chains, the next
		/// highest precedence mechanic in the chain fires (given that the input is satisfied). Each
		/// additional chained mechanic can also chain mechanics, hence recursive chanining. 
		/// Precondidtion: All mechanics must only chain to mechanics of lower precedence. 
		/// </summary>
		private void recursivePrecedenceChaining() {
			//If the root mechanic isn't finished executing, do nothing.
			if (currentMechanics.First != null && !currentMechanics.First.Value.FFInputSatisfied())
				return;

			//Ensure the first input satisfied mechanic is at the top of the list.
			int i;
			for (i = 0; i < mechanics.Count; i++) {
				if (mechanics[i].enabled && mechanics[i].FFInputSatisfied()) {
					if (currentMechanics.First == null || !mechanics[i].Equals (currentMechanics.First.Value)) {
						currentMechanics.AddFirst (mechanics[i]);
						currentMechanics.First.Value.FFStart ();
					}
					break;
				}
			}

			LinkedListNode<Mechanic> chain = recurseChain (currentMechanics.First);
			if (chain != null) {
				//Set the chain to the tail, which will be all the mechanics we want to finish.
				chain = chain.Next;
				LinkedListNode<Mechanic> next;
				while (chain != null) {
					next = chain.Next;
					if (chain.Value.FFFinish())
						currentMechanics.Remove(chain);
					chain = next;
				}
			}
		}

		private LinkedListNode<Mechanic> recurseChain(LinkedListNode<Mechanic> mech) {
			//Go through each mech's chain rules and start any mechanics with inputs
			if (mech == null || mech.Value.chainRules.Count == 0)
				return mech;

			foreach (int chainRule in mech.Value.chainRules) {
				if (mechanics[chainRule].FFInputSatisfied()) {
					//If the next one we expect in the list isn't 
					if (mech.Next == null || !mech.Next.Value.Equals (mechanics[chainRule])) {
						//if mechanic not already in list
						LinkedListNode<Mechanic> node = mech;
						while (node != null && !node.Value.Equals(mechanics[chainRule])) {node = node.Next;}
						if (node == null) {
							//Add the new mechanic into the chain
							currentMechanics.AddAfter(mech, mechanics[chainRule]);
							mechanics[chainRule].FFStart();
							//Debug.Log ("Chaining " + mechanics[chainRule].GetType().Name);
						} else {
							//The mechanic was already in the chain, reposition it. 
							currentMechanics.Remove (node);
							currentMechanics.AddAfter (mech, node);
							//Debug.Log (string.Format ("Re-chaining {0} to after {1}", node.Value.GetType().Name, mech.Value.GetType().Name));
						}

					}
					//Recurse chain the next one
					mech = mech.Next;
					return recurseChain(mech);
				}
			}


			return mech;
		}

		private void applyMechanics() {
			if (currentMechanics.First == null)
				defaultMechanic.FFFixedUpdate ();
			else {
				for (LinkedListNode<Mechanic> mech_n = currentMechanics.First; mech_n != null; mech_n = mech_n.Next) {
					mech_n.Value.FFFixedUpdate();
					if (!mech_n.Value.FFInputSatisfied() )
						if (mech_n.Value.FFFinish() )
							currentMechanics.Remove (mech_n.Value);
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

		private int getPrecedence(Mechanic mech) {
			for (int i = 0; i < mechanics.Count; i++) {
				if (mechanics[i].Equals (mech))
					return i;
			}

			return -1;
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
