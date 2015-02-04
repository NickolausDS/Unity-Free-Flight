using UnityEngine;
using System.Collections;

namespace UnityFreeFlight {

	/// <summary>
	/// Apply flight mechanics on a game object when enabled by the Mode Manager
	/// </summary>
	public class FlightMode : BaseMode {
	
		public FlightInputs flightInputs;
		public FreeFlightAnimationHashIDs hashIDs; 


		public FlightMode (GameObject go) : base(go) {
			flightInputs = new FlightInputs ();
			hashIDs = new FreeFlightAnimationHashIDs ();
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

		public bool enabledGliding = true;
		//Basic gliding input, values in degrees
		public float maxTurnBank = 45.0f;
		public float maxPitch = 20.0f;
		public float directionalSensitivity = 2.0f;
		
		public bool enabledFlapping = true;
		public AudioClip flapSoundClip;
		private AudioSource flapSoundSource;
	//	public float regularFlaptime = 0.5f;
	//	public float minimumFlapTime = 0.2f;
		public float flapStrength = 60.0f;
	//	public float downbeatStrength = 150.0f;
		
		public bool enabledFlaring = false;
		public AudioClip flareSoundClip;
		private AudioSource flareSoundSource;
		//The default pitch (x) we rotate to when we do a flare
		public float flareAngle = 70.0f;
		public float flareSpeed = 3.0f;
		
		public bool enabledDiving = false;
		public AudioClip divingSoundClip;
		private AudioSource divingSoundSource;
	
		public bool enabledLanding = true;
		public AudioClip landingSoundClip;
		private AudioSource landingSoundSource;
		//Max time "standUp" will take to execute.
		public float maxStandUpTime = 2.0f;
		//Speed which "standUp" will correct rotation. 
		public float standUpSpeed = 2.0f;
	
		public bool enabledCrashing = false;
		public float crashSpeed = 40f;
		public AudioClip crashSoundClip;
		private AudioSource crashSoundSource;
	
		public bool enabledWindNoise = true;
		public AudioClip windNoiseClip;
		private AudioSource windNoiseSource;
		public float windNoiseStartSpeed = 20.0f;
		public float windNoiseMaxSpeed = 200.0f;

		public override void startMode () {
			animator.SetBool(hashIDs.flyingBool, true);
			rigidbody.freezeRotation = true;
			rigidbody.isKinematic = false;
		}

		public override void finishMode () {
			flightInputs.resetInputs ();
			rigidbody.freezeRotation = true;
			animator.SetBool (hashIDs.flaringBool, false);
			animator.SetBool(hashIDs.flyingBool, false);
			if (enabledCrashing && flightPhysics.Speed >= crashSpeed) {
				animator.SetTrigger(hashIDs.dyingTrigger);
				//playSound (crashSoundSource);
			} else {
				//playSound (landingSoundSource);
				FreeFlight ff = gameObject.GetComponent<FreeFlight>();
				ff.StartCoroutine (standUp ());
			}		}



		public override void getInputs () {
			flightInputs.getInputs ();
		}

		public override void applyInputs () {
			//HACK -- currently, drag is being fully calculated in flightPhysics.cs, so we don't want the
			//rigidbody adding any more drag. This should change, it's confusing to users when they look at
			//the rigidbody drag. 
			rigidbody.drag = 0.0f;
			//precedence is as follows: flaring, diving, regular gliding flight. This applies if the
			//player provides multiple inputs. Some mechanics can be performed at the same time, such 
			//as flapping while flaring, or turning while diving. 
			
			
			//Flaring takes precedence over everything
			if (enabledFlaring && flightInputs.inputFlaring) {
				flare ();
				if(flightInputs.inputFlap)
					flap ();
			} 
			
			//Diving takes precedence under flaring
			if(enabledDiving && flightInputs.inputDiving && !flightInputs.inputFlaring) {
				dive ();
			} else if (!flightInputs.inputDiving && !flightPhysics.wingsOpen()) {
				//Simulates coming out of a dive
				dive ();
			}
			
			//Regular flight takes last precedence. Do regular flight if not flaring or diving.
			if ( !((enabledDiving && flightInputs.inputDiving) || (enabledFlaring && flightInputs.inputFlaring)) ) {
				flightPhysics.directionalInput(getBank (), getPitch (false), directionalSensitivity);
				//Allow flapping during normal flight
				if (flightInputs.inputFlap)
					flap ();
			}

			if (!flightInputs.inputFlaring)
				animator.SetBool (hashIDs.flaringBool, false);
			if (!flightInputs.inputDiving) {
				animator.SetBool (hashIDs.divingBool, false);
			}

			flightPhysics.doStandardPhysics ();

			
	
			animator.SetFloat (hashIDs.speedFloat, rigidbody.velocity.magnitude);
			animator.SetFloat (hashIDs.angularSpeedFloat, getBank ());
	
		}

		protected override void applyPhysics ()
		{
			flightPhysics.doStandardPhysics ();
		}
		
		
		/// <summary>
		/// Calculates pitch, based on user input and configured pitch parameters.
		/// </summary>
		/// <returns>The pitch in degrees.</returns>
		/// <param name="flare">If set to <c>true</c> calculates pitch of a flare angle.</param>
		protected float getPitch(bool flare) {
			if (flare)
				return flightInputs.inputPitch * maxPitch - flareAngle;
			else
				return flightInputs.inputPitch * maxPitch;
		}
		
		protected float getBank() {
			return flightInputs.inputBank * maxTurnBank;
		}
		
		protected void flap() {
			if(!enabledFlapping) {
				return;
			}
			AnimatorStateInfo curstate = animator.GetCurrentAnimatorStateInfo (0);
			if (curstate.nameHash != hashIDs.flappingState) {
				//playSound (flapSoundSource);
				rigidbody.AddForce (rigidbody.rotation * Vector3.up * flapStrength);
				animator.SetTrigger (hashIDs.flappingTrigger);
			}
		}
		
		protected void flare() {
			if (enabledFlaring) {
				//playSound (flareSoundSource);
				animator.SetBool (hashIDs.flaringBool, true);
				//Flare is the same as directional input, except with exagerated pitch and custom speed. 
				flightPhysics.directionalInput(getBank (), getPitch (true), flareSpeed);
			}
		}
		
		protected void dive() {
			if (enabledDiving) {
				//playSound (divingSoundSource);
				animator.SetBool (hashIDs.divingBool, true);
				flightPhysics.wingFold(flightInputs.inputLeftWingExposure, flightInputs.inputRightWingExposure);
			}
		}
	
		/// <summary>
		/// Straightenes the flight object on landing, by rotating the roll and pitch
		/// to zero over time. Public vars "standUpSpeed" and "maxStandUpTime" can 
		/// be used to tweak behaviour.
		/// </summary>
		/// <returns>The up.</returns>
		protected IEnumerator standUp() {
			//Find the direction the flight object should stand, without any pitch and roll. 
			Quaternion desiredRotation = Quaternion.identity;
			desiredRotation.eulerAngles = new Vector3 (0.0f, rigidbody.rotation.eulerAngles.y, 0.0f);
			//Grab the current time. We don't want 'standUp' to take longer than maxStandUpTime
			float time = Time.time;
	
			rigidbody.rotation = desiredRotation; //Quaternion.Lerp (rigidbody.rotation, desiredRotation, standUpSpeed * Time.deltaTime);
	
			//Break if the player started flying again, or if we've reached the desired rotation (within 5 degrees)
			while (Quaternion.Angle(rigidbody.rotation, desiredRotation) > 5.0f) {
				//Additionally break if we have gone over time
				if (time + maxStandUpTime < Time.time)
					break;
				//Correct the rotation
				rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, desiredRotation, standUpSpeed * Time.deltaTime);
				yield return null;
			}
			yield return null;
		}


	}
}
