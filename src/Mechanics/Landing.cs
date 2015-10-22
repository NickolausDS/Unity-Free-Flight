using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;

namespace UnityFreeFlight {

	[Serializable]
	public class Landing : Mechanic {

		[Header ("Landing")]
		[Tooltip("Animation Controller trigger parameter for Landing Animation")]
		public string landingTrigger = "";
		private int landingHash;
		public AudioClip landingSound;
		[Tooltip("Time in seconds until standup will snap to the correct rotation")]
		public float maxStandUpTime = 2.0f;
		[Tooltip("Speed to slowely correct to upright orientation")]
		public float standUpSpeed = 2.0f;
		//Main Free Flight script is used to start coroutines.
		private FreeFlight mainMonbehaviour;

		[Header ("Crashing")]
		public bool enabledCrashing = false;
		[Tooltip("Animation Controller trigger parameter for Crashing Animation")]
		public string crashingTrigger = "";
		private int crashingHash;
		public AudioClip crashSound;
		[Tooltip ("The speed a crash will happen instead of a landing")]
		public float crashSpeed = 20f;

		[Header ("Sound")]
		public SoundManager soundManager = new SoundManager();

		private FlightPhysics flightPhysics;

		public override void init (GameObject go, System.Object customPhysics) {
			flightPhysics = (FlightPhysics)customPhysics;
			base.init (go);
			soundManager.init (go);
			mainMonbehaviour = gameObject.GetComponent<FreeFlight> ();
			setupAnimation (landingTrigger, ref landingHash);
			setupAnimation (crashingTrigger, ref crashingHash);
		}


		public override void FFStart () {
			//Check for a crash
			if (enabledCrashing && flightPhysics.airspeed > crashSpeed) {
				if (crashingHash != 0)
					animator.SetTrigger(crashingHash);
				soundManager.playSound (crashSound);
			//Do a regular landing otherwise
			} else {
				if (landingHash != 0)
					animator.SetTrigger(landingHash);
				mainMonbehaviour.StartCoroutine (standUp ());
			}
		}

		//Don't do default behavior. 
		public override bool FFFinish () {return true;}
	
		/// <summary>
		/// Straightenes the flight object on landing, by rotating the roll and pitch
		/// to zero over time. Public vars "standUpSpeed" and "maxStandUpTime" can 
		/// be used to tweak behaviour.
		/// </summary>
		/// <returns>The up.</returns>
		private IEnumerator standUp() {
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
