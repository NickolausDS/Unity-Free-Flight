using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	[Serializable]
	public class Flapping : Mechanic {

		[Header("Settings")]
		public string flappingAnimation = "Flapping";
		private int flappingHash;
		public float flapStrength = 20.0f;
		AnimatorStateInfo curstate;
		public AudioClip[] sounds;
		public SoundManager soundManager = new SoundManager();

		public override void init (GameObject go, FlightPhysics fp, FlightInputs fi) {
			base.init (go, fp, fi);
			soundManager.init (go);
			name = "Flapping Mechanic";
			setupAnimation (flappingAnimation, ref flappingHash);
		}

		public override bool FFInputSatisfied () {
			return flightInputs.inputFlap;
		}

		/// <summary>
		/// Override FFStart to do nothing. The Stock Begin() isn't what we want
		/// </summary>
		public override void FFStart () {}

		public override void FFFixedUpdate () {
			if (!isFlapping()) {
				soundManager.playRandomSound(sounds);
				rigidbody.AddForce (rigidbody.rotation * Vector3.up * flapStrength);
				animator.SetTrigger (flappingHash);
			}
		}

		/// <summary>
		/// Since flapping animation is done on a trigger, we want to override the default behavior.
		/// </summary>
		public override bool FFFinish () {
			return true;
		}

		public bool isFlapping () {
			curstate = animator.GetCurrentAnimatorStateInfo (0);
			if (curstate.fullPathHash != flappingHash) {
				return false;
			}
			return true;
		}


	}
}
