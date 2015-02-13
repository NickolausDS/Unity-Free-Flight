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

		private static System.Random randomNumber = new System.Random ();


		public string name = "Anonymous Mechanic"; 
		public bool enabled = true;
		//list of mechanics this mechanic can and can't execute alongside
		//This list is managed by the mechanic manager exclusively.
//		public List<bool> executionPriority; 
		//Animation
		public string animationStateName;
		protected int animationStateHash;

		protected GameObject gameObject;
		protected Rigidbody rigidbody;
		protected Animator animator;
		protected SoundManager soundManager;
		protected FreeFlightPhysics flightPhysics;
		protected FlightInputs flightInputs;

		public AudioClip[] sounds;

		/// <summary>
		/// Special Method, called on OnEnable() of master component. Do any initializing or caching here. 
		/// </summary>
		/// <param name="go">Go.</param>
		/// <param name="sm">Sm.</param>
		public virtual void init(GameObject go, SoundManager sm, FreeFlightPhysics fp, FlightInputs fi) {
			if (!gameObject)
				gameObject = go;
			if (flightPhysics == null)
				flightPhysics = fp;
			if (soundManager == null)
				soundManager = sm;
			if (!animator)
				animator = gameObject.GetComponentInChildren <Animator> ();
			if (!rigidbody)
				rigidbody = gameObject.GetComponent <Rigidbody> ();
			if (flightInputs == null)
				flightInputs = fi;
		}

		/// <summary>
		/// Check if all the inputs necessary for firing this mechanic are set.
		/// </summary>
		/// <returns><c>true</c>, if player is pressing all buttons to fire mechanic, <c>false</c> otherwise.</returns>
		public virtual bool FFInputSatisfied () { return false; }


		/// <summary>
		/// Do prep work, usually applying any settings changed by the user in the inspector 
		/// </summary>
		/// <param name="animator">Animator.</param>
		/// <param name="rigidbody">Rigidbody.</param>
		public virtual void FFStart () {
			//usually what we want for start()
			animationStateHash = Animator.StringToHash (animationStateName);
		}

		/// <summary>
		/// Do anything needed to start the mechanic. Set the animation bools or triggers here, and do any
		/// other prep work needed. 
		/// </summary>
		public virtual void FFBegin () {
			//usually what we want for begin()
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
		/// Plays a random sound from the list provided.
		/// </summary>
		public virtual void playSound() {
			if (sounds.Length > 0) {
				//pick a random sound from the list
				soundManager.playSound(sounds[randomNumber.Next (sounds.Length)]);
			}

		}
	}
}
