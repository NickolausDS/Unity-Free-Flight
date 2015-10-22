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

		public bool usePhysics;
		public bool alwaysApplyPhysics;

		protected GameObject gameObject;

		[NonSerialized]
		public List<Mechanic> mechanics = new List<Mechanic> ();
		[NonSerialized]
		public Mechanic defaultMechanic;
		[NonSerialized]
		public Mechanic finishMechanic;
		[NonSerialized]
		public LinkedList<Mechanic> currentMechanics = new LinkedList<Mechanic> ();


		public List<string> mechanicTypeNames = new List<string> ();
		public string defaultMechanicTypeName;
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
		}

		/// <summary>
		/// Do initialization for the modes mechanics. Mode Mechanics **MUST** be instantiated **ONLY** if they are null, 
		/// otherwise serialized data will be overwritten with default values. The same goes for inputs and physics.
		/// </summary>
		/// <param name="modeMechanics">Mode mechanics.</param>
		/// <param name="modeInputs">Mode inputs.</param>
		/// <param name="customPhysics">Custom physics.</param>
		public void setupMechanics(PolymorphicSerializer modeMechanics,System.Object customPhysics = null) {

			if (modeMechanics == null)
				throw new NullReferenceException ("Mode failed setupMechanics(): 'Mode mechanics' must be instantiated or " +
					"deserialized before calling this method.");
			
			modeMechanics.load<Mechanic> (defaultMechanicTypeName, ref defaultMechanic);
			modeMechanics.load<Mechanic> (mechanicTypeNames, ref mechanics);
			modeMechanics.load<Mechanic> (finishMechanicTypeName, ref finishMechanic);

			foreach (Mechanic mech in mechanics) {
				try {
				mech.init (gameObject, customPhysics);
				} catch {
					Debug.LogError(string.Format ("{0} in {1} failed initialization", mech.name, name));
					throw;
				}
			}
			
			if (defaultMechanic != null) {				
				try {
					defaultMechanic.init (gameObject, customPhysics);
				} catch {
					Debug.LogError(string.Format ("{0} in {1} failed initialization", defaultMechanic.name, name));
					throw;
				}
			} else {
				Debug.LogError (string.Format ("({0}): Default Mechanic cannot be null!", name));
			}
			
			if (finishMechanic != null) {
				try {
					finishMechanic.init (gameObject, customPhysics);
				} catch {
					Debug.LogError(string.Format ("{0} in {1} failed initialization", finishMechanic.name, name));
					throw;
				}
			}

		}

		public virtual void applyInputs () {
			try {
				applyMechanics ();
			} catch (Exception e) {
				Debug.LogException(e);
			}

			if (usePhysics)
				applyPhysics ();
		}

		/// <summary>
		/// Tells the manager to gather inputs from the player.
		/// This will happen in a frame-by-frame update time scale.
		/// </summary>
		public virtual void getInputs() {
			applyMechanicPrecedence ();
			applyrecursivePrecedenceChaining ();
		}

		/// <summary>
		/// Tweak anything that needs to happen before this modes mechanics run. 
		/// </summary>
		public virtual void startMode () {
			defaultMechanic.FFStart ();
		}

		/// <summary>
		/// Clean up any settings before this mode ends, including resetting things
		/// to be in a healthy state when this mode is run again. Please be nice to 
		/// the other modes, and reset any crazy values you temporarily set.
		/// </summary>
		public virtual void finishMode () {

			for(LinkedListNode<Mechanic> node = currentMechanics.Last; node != null; node = node.Previous) {
				node.Value.FFFinish();
			}

			defaultMechanic.FFFinish ();

			if (finishMechanic != null) {
				finishMechanic.FFStart ();
				finishMechanic.FFFinish ();	
			}
		}
		
		protected virtual void applyPhysics() {
			usePhysics = false;
			Debug.LogError(name + " is not equipped with custom physics. Disabling...");
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
				if (mech.enabled && mech.FFInputSatisfied()) {
					if (currentMechanics.First == null || !mech.Equals (currentMechanics.First.Value)) {
						currentMechanics.AddFirst (mech);
						currentMechanics.First.Value.FFStart ();
					}
					break;
				}
			}		
		}

		/// <summary>
		/// The highest precendence mechanic runs first. If the mechanic contains chains, the next
		/// highest precedence mechanic in the chain fires (given that the input is satisfied). Each
		/// additional chained mechanic can also chain mechanics, hence recursive chanining. 
		/// Precondidtion: All mechanics must only chain to mechanics of lower precedence. 
		/// </summary>
		private void applyrecursivePrecedenceChaining() {
			//If the root mechanic isn't finished executing, do nothing.
			if (currentMechanics.First != null && !currentMechanics.First.Value.FFInputSatisfied())
				return;

			//Add any chained mechanics
			LinkedListNode<Mechanic> chain = recursiveChain (currentMechanics.First);

			//Remove the tail
			if (chain != null)
				removeTail (chain.Next);
		}

		/// <summary>
		/// Recursive Chain adds and starts additional mechanics according to the first mechanics
		/// chain rules (also requires the mechanic to satisfy input). All new mechanics are added
		/// directly after the main. If the main mechanic is swapped for another, and still allows
		/// old mechanics in the chain, they are carried over. If the new main DOES NOT allow some 
		/// old mechanics, they REMAIN IN THE TAIL OF THE CHAIN. This method returns the last valid 
		/// mechanic in the chain, so the old tail mechanics are accessible via chain.Next.
		/// </summary>
		/// <returns>The last chained mechanic.</returns>
		/// <param name="mech">Mech.</param>
		private LinkedListNode<Mechanic> recursiveChain(LinkedListNode<Mechanic> chain) {
			//Go through each mech's chain rules and start any mechanics with inputs
			if (chain == null || chain.Value.chainRules.Count == 0)
				return chain;

			foreach (int chainRule in chain.Value.chainRules) {
				if (mechanics[chainRule].FFInputSatisfied()) {
					//If the next one we expect in the list isn't 
					if (chain.Next == null || !chain.Next.Value.Equals (mechanics[chainRule])) {
						//if mechanic not already in list
						LinkedListNode<Mechanic> node = chain;
						while (node != null && !node.Value.Equals(mechanics[chainRule])) {node = node.Next;}
						if (node == null) {
							//Add the new mechanic into the chain
							currentMechanics.AddAfter(chain, mechanics[chainRule]);
							mechanics[chainRule].FFStart();
							//Debug.Log ("Chaining " + mechanics[chainRule].GetType().Name);
						} else {
							//The mechanic was already in the chain, reposition it. 
							currentMechanics.Remove (node);
							currentMechanics.AddAfter (chain, node);
							//Debug.Log (string.Format ("Re-chaining {0} to after {1}", node.Value.GetType().Name, mech.Value.GetType().Name));
						}

					}
					//Recurse chain the next one
					chain = chain.Next;
					return recursiveChain(chain);
				}
			}
			return chain;
		}

		private void removeTail(LinkedListNode<Mechanic> chain) {
			LinkedListNode<Mechanic> next;
			while (chain != null) {
				next = chain.Next;
				if (chain.Value.FFFinish())
					currentMechanics.Remove(chain);
				chain = next;
			}
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

	}

}
