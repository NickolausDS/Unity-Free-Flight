using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityFreeFlight;


namespace UnityFreeFlight {
	/// <summary>
	/// A Free Flight mechanic is any in-flight action the player can take. These 
	/// include flapping, diving, cawing, roaring, pooping, or any other action 
	/// the flight object should take in flight. 
	/// </summary>
	[Serializable]
	public class Mechanic {

		[HideInInspector]
		public string name = "Anonymous Mechanic"; 
		[HideInInspector]
		public bool enabled = true;
		//list of mechanics this mechanic can and can't execute alongside
		//This list is managed by the mechanic manager exclusively.
//		public List<bool> executionPriority; 
		//Animation
		public string animationStateName;
		protected int animationStateHash;

		public string input = "";

		protected GameObject gameObject;
		protected Rigidbody rigidbody;
		protected Animator animator;
		protected FlightPhysics flightPhysics;
		protected FlightInputs flightInputs;

		/// <summary>
		/// Special Method, called on OnEnable() of master component. Do any initializing or caching here. 
		/// </summary>
		/// <param name="go">Go.</param>
		/// <param name="sm">Sm.</param>
		public virtual void init(GameObject go, FlightPhysics fp, FlightInputs fi) {
			gameObject = go;
			flightPhysics = fp;
			animator = gameObject.GetComponentInChildren <Animator> ();
			rigidbody = gameObject.GetComponent <Rigidbody> ();
			flightInputs = fi;
		}

		/// <summary>
		/// Check if all the inputs necessary for firing this mechanic are set.
		/// </summary>
		/// <returns><c>true</c>, if player is pressing all buttons to fire mechanic, <c>false</c> otherwise.</returns>
		public virtual bool FFInputSatisfied () { 
			if (input != "")
				return Input.GetKeyDown (input); 
			return false;
		}


		/// <summary>
		/// Do prep work, usually applying any settings changed by the user in the inspector 
		/// </summary>
		public virtual void FFStart () {
			animator.SetBool (animationStateHash, true);
		}

		/// <summary>
		/// Apply constant changes to the flight object under the fixed update time scale. This includes
		/// any ongoing physics related to this mechanic (excluding general flight physics).
		/// </summary>
		/// <param name="animator">Animator.</param>
		/// <param name="rigidbody">Rigidbody.</param>
		public virtual void FFFixedUpdate () {}


		/// <summary>
		/// The mechanic inputs are no longer satisfied, do anything required to stop animation, such as setting bools to 
		/// false. Clean up and reset any temporary variables or timers. 
		/// </summary>
		/// <param name="animator">Animator.</param>
		/// <param name="rigidbody">Rigidbody.</param>
		public virtual bool FFFinish () {
			animator.SetBool (animationStateHash, false);
			return true;
		}

		/// <summary>
		/// Hash the string, and also check whether the hash exists within
		/// the animation controller. If no matching state exists within 
		/// the controller, the hash is set to zero. 
		/// </summary>
		/// <param name="animString">Animation string.</param>
		/// <param name="animHash">Animation hash.</param>
		public void setupAnimation(string animString, int layer, ref int animHash) {
			if (animator != null && animString != null) {
				animHash = Animator.StringToHash (animString);
				if (!animator.HasState (layer, animHash))
					animHash = 0;
			}
		}


	}
}
