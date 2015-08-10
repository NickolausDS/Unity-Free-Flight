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
		[Tooltip ("The speed bank returns to normal")]
		public float bankSensitivity = 2.0f;
		[Tooltip ("The speed pitch returns to normal")]
		public float pitchSensitivity = 2.0f;
		[Tooltip ("The speed pitch and bank adjustments to their default positions")]
		public float sensitivity = 2.0f;
		[Tooltip ("Adjust the pitch based on the angle of attack in flight. (allows stalls and dive recovery)")]
		public bool pitchByAOA = true;
		[Tooltip ("The default pitch when doing normal gliding")]
		public float defaultPitch = -5f;
		//I have no idea why this would ever be changed, but I've included it for 'completion'. 
		[Tooltip ("The default resting position for bank. (Why would you want this non-zero!?)")]
		public float defaultBank = 0f;
		//variable where we'll store pitch adjustments
		private float pitchAdjustment;
		
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
			flightPhysics.addBank (defaultBank, sensitivity);
			pitchAdjustment = (pitchByAOA? -flightPhysics.angleOfAttack : 0f) + defaultPitch;
			flightPhysics.addPitch (pitchAdjustment, sensitivity);

			applyWindNoise ();
		}

		public override bool FFFinish () {
			animator.SetBool (glidingHash, false);
			return true;
		}
		
		private void applyWindNoise() {
			
			if (!windNoiseClip)
				return;
			
			AudioSource windNoiseSource = soundManager.audioSource;
			if (!windNoiseSource) {
				Debug.LogError ("Wind source noise has clip but not source!");
				return;
			}

			if (flightPhysics.airspeed > windNoiseStartSpeed) {
				float volume = Mathf.Clamp (flightPhysics.airspeed / (windNoiseStartSpeed + windNoiseMaxSpeed), 0.0f, 1.0f);
				windNoiseSource.volume = volume;
				//We want pitch to pick up at about half the volume
				windNoiseSource.pitch = Mathf.Clamp (0.9f + flightPhysics.airspeed / 2.0f / (windNoiseStartSpeed + windNoiseMaxSpeed), 0.9f, 1.5f);
				//Use this to see how values are applied at various speeds.
				//Debug.Log (string.Format ("Vol {0}, pitch {1}", audio.volume, audio.pitch));
				if (! windNoiseSource.isPlaying) 
					soundManager.playSound(windNoiseClip);
					//windNoiseSource.Play ();
			} else {
				windNoiseSource.Stop ();
			}
			
		}
	
	}
}

