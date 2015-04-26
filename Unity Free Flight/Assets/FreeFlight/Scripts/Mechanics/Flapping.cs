using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	[Serializable]
	public class Flapping : Mechanic {

		[Header("Animation")]
		public string flappingParameter = "Flapping";
		public string flappingState = "Base Layer.Flapping";
		private int paramHash;
		private int stateHash;

		[Header("Sound")]
		public AudioClip[] sounds;
		public SoundManager soundManager = new SoundManager();

		[Header("General")]
		[Tooltip("Units/second of force applied to object")]
		public float flapStrength = 5.0f;
		[Tooltip("Time in seconds before any flap force is applied")]
		public float lagTime = 0f;
		[Tooltip("Duration in seconds of flap (not connected to animation length)")]
		public float duration = .5f;
		private float startTime;

		public override void init (GameObject go, FlightPhysics fp, FlightInputs fi) {
			base.init (go, fp, fi);
			soundManager.init (go);
			name = "Flapping Mechanic";
			setupAnimation (flappingParameter, ref paramHash);
			stateHash = Animator.StringToHash (flappingState);
			if (!animator.HasState (0, stateHash)) {
				Debug.LogWarning ("Flapping: Animation Controller doesn't appear to have the '" + flappingState + "' state.");
			}
			
		}

		public override bool FFInputSatisfied () {
			return flightInputs.inputFlap;
		}

		/// <summary>
		/// Override FFStart to do nothing. The Stock Begin() isn't what we want
		/// </summary>
		public override void FFStart () {}

		public override void FFFixedUpdate () {
//			Debug.Log (curstate.shortNameHash + " : " + Animator.StringToHash (flappingAnimation));
			if (animator.GetNextAnimatorStateInfo(0).fullPathHash != stateHash &&
			    animator.GetCurrentAnimatorStateInfo(0).fullPathHash != stateHash &&
			    FFInputSatisfied()
			    ) {
					soundManager.playRandomSound(sounds);
					animator.SetTrigger (paramHash);
					startTime = Time.time;
			} 

			applyFlapForce ();
		}

		/// <summary>
		/// Since flapping animation is done on a trigger, we want to override the default behavior.
		/// </summary>
		public override bool FFFinish () {
			return isDoneFlapping();
		}

		public bool isDoneFlapping() {
			if (startTime + duration + lagTime < Time.time)
				return true;
			return false;
		}

		public void applyFlapForce() {
			if (startTime + lagTime < Time.time && startTime + duration + lagTime > Time.time) {
				rigidbody.AddForce (getFlapForce (0f));
			}
		}

		/// <summary>
		/// Get the force vector for the flapping force (including direction and strength of flap)
		/// </summary>
		/// <returns>The flap force.</returns>
		/// <param name="angle">direction of flap. 0 = thrusts forward, 90 = thrusts stright up.</param>
		public Vector3 getFlapForce(float angle) {
			return rigidbody.rotation * Quaternion.AngleAxis (angle, Vector3.right) * Vector3.forward * flapStrength;
		}


	}
}
