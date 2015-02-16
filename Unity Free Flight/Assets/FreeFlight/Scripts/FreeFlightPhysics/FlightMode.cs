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

		public List<Mechanic> mechanics;
		public Mechanic defaultMechanic;
		private Mechanic currentMechanic = null;
		//A mechanic defined explicity for testing purposes


		public override void init (GameObject go, SoundManager sm) {
			base.init (go, sm);
			flightInputs = new FlightInputs ();
			mechanics = new List<Mechanic> ();
			mechanics.Add (new Flapping ());
			mechanics.Add (new Flaring ());
			mechanics.Add (new Diving ());
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

		public override void applyInputs () {

			applyMechanicPrecedence ();

			applyMechanic ();

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


//		public override void applyInputs () {
			//HACK -- currently, drag is being fully calculated in flightPhysics.cs, so we don't want the
			//rigidbody adding any more drag. This should change, it's confusing to users when they look at
			//the rigidbody drag. 
//			rigidbody.drag = 0.0f;
			//precedence is as follows: flaring, diving, regular gliding flight. This applies if the
			//player provides multiple inputs. Some mechanics can be performed at the same time, such 
			//as flapping while flaring, or turning while diving. 
			
			
//			//Flaring takes precedence over everything
//			if (enabledFlaring && flightInputs.inputFlaring) {
//				flare ();
//				if(flightInputs.inputFlap)
//					flap ();
//			} 
//			
//			//Diving takes precedence under flaring
//			if(enabledDiving && flightInputs.inputDiving && !flightInputs.inputFlaring) {
//				dive ();
//			} else if (!flightInputs.inputDiving && !flightPhysics.wingsOpen()) {
//				//Simulates coming out of a dive
//				dive ();
//			}
//			
//			//Regular flight takes last precedence. Do regular flight if not flaring or diving.
//			if ( !((enabledDiving && flightInputs.inputDiving) || (enabledFlaring && flightInputs.inputFlaring)) ) {
//				flightPhysics.directionalInput(getBank (), getPitch (false), directionalSensitivity);
//				//Allow flapping during normal flight
//				if (flightInputs.inputFlap)
//					flap ();
//			}
//
//			if (!flightInputs.inputFlaring)
//				animator.SetBool (hashIDs.flaringBool, false);
//			if (!flightInputs.inputDiving) {
//				animator.SetBool (hashIDs.divingBool, false);
//			}
//
//			flightPhysics.doStandardPhysics ();
//
//			
//	
//			animator.SetFloat (hashIDs.speedFloat, rigidbody.velocity.magnitude);
//			animator.SetFloat (hashIDs.angularSpeedFloat, getBank ());
//	
//			applyWindNoise ();

//		}

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
