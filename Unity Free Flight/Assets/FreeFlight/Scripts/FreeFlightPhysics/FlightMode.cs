using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityFreeFlight;

namespace UnityFreeFlight {

	/// <summary>
	/// Apply flight mechanics on a game object when enabled by the Mode Manager
	/// </summary>
	[Serializable]
	public class FlightMode : BaseMode {
	
		public FlightInputs _flightInputs;
		public FlightInputs flightInputs { 
			get {
				if (_flightInputs == null)
					_flightInputs = new UnityFreeFlight.FlightInputs ();
				return _flightInputs;
			}
			set { _flightInputs = value;}
		} 
		//A mechanic defined explicity for testing purposes

		/// <summary>
		/// Special mechanics need to be explicitly defined. This is (a hack) because
		/// Unity dosen't support polymorphic serialization for basic classes, 
		/// and doing so with scriptable objects would result in a stupid user
		/// experience.
		/// </summary>
		public Flapping flapping = new Flapping ();
		public Flaring flaring = new Flaring ();
		public Diving diving = new Diving ();
		public Gliding gliding = new Gliding ();


		public override void init (GameObject go, SoundManager sm) {
			base.init (go, sm);
			name = "Flight Mode";

			if (flightInputs == null)
				flightInputs = new FlightInputs ();
			if (mechanics == null)
				mechanics = new List<Mechanic> ();

			foreach (Mechanic mech in mechanics) {
				mech.init (go, sm, flightPhysics, flightInputs);
				mech.FFStart ();
			}

			defaultMechanic = new Gliding();
			defaultMechanic.init (go, sm, flightPhysics, flightInputs);
			defaultMechanic.FFStart ();

			currentMechanic = null;
		}



		private CreatureFlightPhysics _flightPhysics;
		public CreatureFlightPhysics flightPhysics {
			get {
				if (_flightPhysics == null) {
					_flightPhysics = new CreatureFlightPhysics(rigidbody);
				}
				return _flightPhysics;
			}
			set { _flightPhysics = value;}
		}
//	
//		public bool enabledLanding = true;
//		public AudioClip landingSoundClip;
//		//private AudioSource landingSoundSource;
//		//Max time "standUp" will take to execute.
//		public float maxStandUpTime = 2.0f;
//		//Speed which "standUp" will correct rotation. 
//		public float standUpSpeed = 2.0f;
//	
//		public bool enabledCrashing = false;
//		public float crashSpeed = 40f;
//		public AudioClip crashSoundClip;
//		//private AudioSource crashSoundSource;

		public override void startMode () {
//			soundManager.setupSound (flapSoundClip);
//			soundManager.setupSound (windNoiseClip);

			//HACK -- currently, drag is being fully calculated in flightPhysics.cs, so we don't want the
			//rigidbody adding any more drag. This should change, it's confusing to users when they look at
			//the rigidbody drag. 
			rigidbody.drag = 0.0f;

			rigidbody.freezeRotation = true;
			rigidbody.isKinematic = false;

			defaultMechanic.FFBegin ();
			currentMechanic = null;

		}
//
		public override void finishMode () {
			flightInputs.resetInputs ();
			if (currentMechanic != null)
				currentMechanic.FFFinish ();
			defaultMechanic.FFFinish ();
//			if (enabledCrashing && flightPhysics.Speed >= crashSpeed) {
//				animator.SetTrigger(hashIDs.dyingTrigger);
//				//playSound (crashSoundSource);
//			} else {
//				//playSound (landingSoundSource);
//				FreeFlight ff = gameObject.GetComponent<FreeFlight>();
//				ff.StartCoroutine (standUp ());
//			}		
		}

		public override void getInputs () {
			flightInputs.getInputs ();
		}

		protected override void applyPhysics ()
		{
			flightPhysics.doStandardPhysics ();
		}


//	
//		/// <summary>
//		/// Straightenes the flight object on landing, by rotating the roll and pitch
//		/// to zero over time. Public vars "standUpSpeed" and "maxStandUpTime" can 
//		/// be used to tweak behaviour.
//		/// </summary>
//		/// <returns>The up.</returns>
//		protected IEnumerator standUp() {
//			//Find the direction the flight object should stand, without any pitch and roll. 
//			Quaternion desiredRotation = Quaternion.identity;
//			desiredRotation.eulerAngles = new Vector3 (0.0f, rigidbody.rotation.eulerAngles.y, 0.0f);
//			//Grab the current time. We don't want 'standUp' to take longer than maxStandUpTime
//			float time = Time.time;
//	
//			rigidbody.rotation = desiredRotation; //Quaternion.Lerp (rigidbody.rotation, desiredRotation, standUpSpeed * Time.deltaTime);
//	
//			//Break if the player started flying again, or if we've reached the desired rotation (within 5 degrees)
//			while (Quaternion.Angle(rigidbody.rotation, desiredRotation) > 5.0f) {
//				//Additionally break if we have gone over time
//				if (time + maxStandUpTime < Time.time)
//					break;
//				//Correct the rotation
//				rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, desiredRotation, standUpSpeed * Time.deltaTime);
//				yield return null;
//			}
//			yield return null;
//		}
//


	}
}
