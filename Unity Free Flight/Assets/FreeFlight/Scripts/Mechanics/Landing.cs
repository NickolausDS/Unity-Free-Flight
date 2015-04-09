using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;

namespace UnityFreeFlight {

	[Serializable]
	public class Landing : Mechanic {

		//private AudioSource landingSoundSource;
		//Max time "standUp" will take to execute.
		public float maxStandUpTime = 2.0f;
		//Speed which "standUp" will correct rotation. 
		public float standUpSpeed = 2.0f;
		private FreeFlight mainMonbehaviour;

		//	
		//		public bool enabledCrashing = false;
		//		public float crashSpeed = 40f;
		//		public AudioClip crashSoundClip;
		//		//private AudioSource crashSoundSource;

		public override void init (GameObject go, SoundManager sm, FlightPhysics fp, FlightInputs fi) {
			base.init (go, sm, fp, fi);
			mainMonbehaviour = gameObject.GetComponent<FreeFlight> ();
		}


		public override void FFStart () {
			if (animationStateHash != 0)
				animator.SetTrigger(animationStateHash);
			mainMonbehaviour.StartCoroutine (standUp ());
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
