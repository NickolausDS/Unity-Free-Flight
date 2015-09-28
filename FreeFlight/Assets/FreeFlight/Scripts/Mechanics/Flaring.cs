using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {
	
	[Serializable]
	public class Flaring : Mechanic {

		[Header("Animation")]
		public string flaringAnimation = "Flaring";
		private int flaringHash;

		[Header("Sound")]
		public AudioClip flareSoundClip;
		public SoundManager soundManager = new SoundManager();

		[Header("General")]
		//The default pitch (x) we rotate to when we do a flare
		public float flareAngle = 70.0f;
		public float flareSpeed = 3.0f;
		
		private FlightPhysics flightPhysics;
		private FlightInputs flightInputs;
		
		public override void init (GameObject go, System.Object customPhysics, Inputs inputs) {
			flightPhysics = (FlightPhysics)customPhysics;
			flightInputs = (FlightInputs)inputs;
			base.init (go);
			name = "Flaring Mechanic";
			setupAnimation (flaringAnimation, ref flaringHash);
		}
		
		public override bool FFInputSatisfied () {
			return flightInputs.inputFlaring;
		}

		public override void FFStart () {
			animator.SetBool (flaringHash, true);
			soundManager.playSound (flareSoundClip);
		} 
		
		public override void FFFixedUpdate () {
			flightPhysics.addPhysicsPitch (flareAngle, flareSpeed, this);
		}

		public override bool FFFinish () {
			animator.SetBool (flaringHash, false);
			flightPhysics.releasePhysicsRotation (this);
			return true;
		}
		
	}
}
