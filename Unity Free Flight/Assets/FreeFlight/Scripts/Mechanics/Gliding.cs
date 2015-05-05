using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	[Serializable]
	public class Gliding : Mechanic {

		[Header("Animation")]
		public string glidingAnimation = "Gliding";
		private int glidingHash;

		[Header("Sound")]
		public float windNoiseStartSpeed = 20.0f;
		public float windNoiseMaxSpeed = 200.0f;
		public AudioClip windNoiseClip;
		public SoundManager soundManager = new SoundManager();

		[Header("General")]
		//Basic gliding input, values in degrees
		public float maxBank = 45.0f;
		public float bankSensitivity = 2.0f;
		public float maxPitch = 20.0f;
		public float pitchSensitivity = 2.0f;
		
		public override void init (GameObject go, FlightPhysics fm, FlightInputs fi) {
			base.init (go, fm, fi);
			setupAnimation (glidingAnimation, ref glidingHash);
		}
		
		public override bool FFInputSatisfied () {
			return true;
		}

		public override void FFStart () {
			animator.SetBool (glidingHash, true);
		}
		
		public override void FFFixedUpdate () {
			flightPhysics.addBank (flightInputs.inputBank * maxBank, bankSensitivity);
			flightPhysics.addPitch (flightInputs.inputPitch * maxPitch + flightPhysics.angleOfAttack + 5f, pitchSensitivity);

		}

		public override bool FFFinish () {
			animator.SetBool (glidingHash, false);
			return true;
		}
		
		private void applyWindNoise() {
			
			if (!windNoiseClip)
				return;
			
//			AudioSource windNoiseSource = soundManager.getSource (windNoiseClip);
//			if (!windNoiseSource) {
//				Debug.LogError ("Wind source noise has clip but not source!");
//				return;
//			}
			
//			if (flightPhysics.airspeed > windNoiseStartSpeed) {
//				float volume = Mathf.Clamp (flightPhysics.airspeed / (windNoiseStartSpeed + windNoiseMaxSpeed), 0.0f, 1.0f);
//				windNoiseSource.volume = volume;
//				//We want pitch to pick up at about half the volume
//				windNoiseSource.pitch = Mathf.Clamp (0.9f + flightPhysics.airspeed / 2.0f / (windNoiseStartSpeed + windNoiseMaxSpeed), 0.9f, 1.5f);
//				//Use this to see how values are applied at various speeds.
//				//Debug.Log (string.Format ("Vol {0}, pitch {1}", audio.volume, audio.pitch));
//				if (! windNoiseSource.isPlaying) 
//					windNoiseSource.Play ();
//			} else {
//				windNoiseSource.Stop ();
//			}
			
		}
	
	}
}

