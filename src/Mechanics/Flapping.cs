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
	public class Flapping : Mechanic  {

		[Header("Inputs")]
		public string button = "Jump";

		[Header("Animation")]
		public string flappingParameter = "Flapping";
		public string flappingState = "Base Layer.Flapping";
		private int paramHash;
		private int stateHash;
		[Tooltip ("Use the Flapping State Machine Behaviour instead of normal detection. Results in smoother flaps, but needs to be set on" +
			"the Flapping state in the animation controller")]
		public bool useSMB = false;
		private FlappingSMB flappingSMB;

		[Header("Sound")]
		public AudioClip[] sounds;
		public SoundManager soundManager = new SoundManager();

		[Header("General -- Flight")]
		[Tooltip("Units/second of force applied to object")]
		public float flapStrength = 10.0f;
		[Tooltip("Time in seconds before any flap force is applied")]
		public float lagTime = 0f;
		[Tooltip("Duration in seconds of flap (not connected to animation length)")]
		public float duration = .5f;
		private float startTime;
		[Tooltip("The pitch angle for flapping.")]
		public float pitchAngle = 5f;

		//maximum angle to apply downward force. 90 is down. Private, because I don't think people will
		//want to change this.
		private float maxAngle = 90f;
		[Header("General -- Takeoff Flapping")]
		[Tooltip("The maximum airspeed for vectoring lift downwards. Higher numbers make easier takeoff")]
		public float maxVectoring = 4f;
		[Tooltip("The strength of a downward vectored flap compared to a forward vectored flap.")]
		public float takeoffStrengthMult = 30f;

		private FlightPhysics flightPhysics;

		public override void init (GameObject go, System.Object customPhysics) {
			flightPhysics = (FlightPhysics)customPhysics;
			base.init (go);
			soundManager.init (go);
			name = "Flapping Mechanic";
			setupAnimation (flappingParameter, ref paramHash);
			stateHash = Animator.StringToHash (flappingState);
			if (!animator.HasState (0, stateHash)) {
				Debug.LogWarning ("Flapping: Animation Controller doesn't appear to have the '" + flappingState + "' state.");
			}
			setupSMB (ref useSMB, ref flappingSMB);

		}

		public override bool FFInputSatisfied () {
			return Input.GetButton (button);
		}

		/// <summary>
		/// Override FFStart to do nothing. The Stock Begin() isn't what we want
		/// </summary>
		public override void FFStart () {

			//If the user has checked or unchecked the option, reset the behaviour. This allows for dynamic 
			//setting of state machine behaviours in-game.
			if (useSMB && flappingSMB == null) {
				Debug.Log ("Enabling Flapping Animation State Machine Behaviour.");
				setupSMB (ref useSMB, ref flappingSMB);
			} else if (!useSMB && flappingSMB != null) {
				Debug.Log ("Disabling Flapping Animation State Machine Behaviour.");
				setupSMB (ref useSMB, ref flappingSMB);
			}
		}

		public override void FFFixedUpdate () {

			if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != stateHash && Input.GetButton (button)) {
				animator.SetTrigger (paramHash);
				if (!useSMB)
					flap ();
			}

			applyFlapForce ();
		}

		/// <summary>
		/// Since flapping animation is done on a trigger, we want to override the default behavior.
		/// </summary>
		public override bool FFFinish () {
			return isDoneFlapping();
		}

		public void flap() {
			soundManager.playRandomSound(sounds);
			startTime = Time.time;
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
				float strength = angle / maxAngle * takeoffStrengthMult + flapStrength * flightPhysics.wingArea;
				rigidbody.AddForce (getFlapForce (angle, strength));
				flightPhysics.addPitch (pitchAngle, 4);
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

		private void setupSMB(ref bool useSMBStatus, ref FlappingSMB fsmb) {
			if (useSMBStatus) {
				fsmb = animator.GetBehaviour<FlappingSMB> ();
				if (fsmb != null)
					fsmb.flap = flap;
				else {
					Debug.LogError ( string.Format ("Free Flight: Flapping -- 'useSMB' was checked, but no attached object was found. " +
						"Please add 'FlappingSMB' to the flapping state in the animation controller" +
					    " for object {0}.", gameObject.name));
					useSMBStatus = false;
				}
			} else if (fsmb != null) {
				fsmb.flap = null;
				fsmb = null;
			}

		}


	}
}
