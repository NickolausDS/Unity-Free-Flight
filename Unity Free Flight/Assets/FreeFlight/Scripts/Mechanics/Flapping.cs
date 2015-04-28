using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	/// <summary>
	/// The flapping mechanic provides thrust and lift for flight objects. It supports multiple sounds, animation, and
	/// some general settings with regard to flap behavior. 
	/// </summary>
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
		//maximum angle to apply downward force. 90 is down. Private, because I don't think people will
		//want to change this.
		private float maxAngle = 90f;
		[Tooltip("The maximum airspeed for vectoring lift downwards. Higher numbers make easier takeoff")]
		public float maxVectoring = 4f;
		[Tooltip("The strength of a downward vectored flap compared to a forward vectored flap.")]
		public float takeoffStrengthMult = 1.5f;

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

		/// <summary>
		/// Check if the physics behind the flap are done. This is not conneceted to the animation, 
		/// and must be synced manually.
		/// </summary>
		/// <returns><c>true</c>, if done flapping , <c>false</c> otherwise.</returns>
		public bool isDoneFlapping() {
			if (startTime + duration + lagTime < Time.time)
				return true;
			return false;
		}

		/// <summary>
		/// Apply flapping force, which won't execute until "lag time" has passed, and will only
		/// execute for as long as "duration" lasts. Total time = lag time + duration. 
		/// </summary>
		public void applyFlapForce() {
			if (startTime + lagTime < Time.time && startTime + duration + lagTime > Time.time) {
				float scale = maxAngle / maxVectoring;
				float angle = Mathf.Clamp ((maxVectoring - flightPhysics.airspeed) * scale, 0.1f, maxAngle); 
				float strength = angle / maxAngle * takeoffStrengthMult + flapStrength;
				rigidbody.AddForce (getFlapForce (angle, strength));
			}
		}

		/// <summary>
		/// Get the force vector for the flapping force (including direction and strength of flap)
		/// </summary>
		/// <returns>The flap force.</returns>
		/// <param name="angle">direction of flap. 0 = thrusts forward, 90 = thrusts stright up.</param>
		/// <param name="strength>strength of flap.</para>">
		public Vector3 getFlapForce(float angle, float strength) {
			return rigidbody.rotation * Quaternion.AngleAxis (angle, Vector3.left) * Vector3.forward * strength;
		}


	}
}
