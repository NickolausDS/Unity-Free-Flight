﻿using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	[Serializable]
	public class Gliding : Mechanic {
		
//		public bool enabledGliding = true;
		//Basic gliding input, values in degrees
		public float maxTurnBank = 45.0f;
		public float maxPitch = 20.0f;
		public float directionalSensitivity = 2.0f;

//		public bool enabledWindNoise = true;
		public AudioClip windNoiseClip;
		//private AudioSource windNoiseSource;
		public float windNoiseStartSpeed = 20.0f;
		public float windNoiseMaxSpeed = 200.0f;
		
		public override void init (GameObject go, SoundManager sm, FlightPhysics fm, FlightInputs fi) {
			base.init (go, sm, fm, fi);
			name = "Gliding";
			animationStateName = "Flying";
			animationStateHash = Animator.StringToHash (animationStateName);
		}
		
		public override bool FFInputSatisfied () {
			return true;
		}
		
		public override void FFFixedUpdate ()
		{
			directionalInput(getBank (), getPitch (false), directionalSensitivity);
		}

		public void directionalInput(float bank, float pitch, float sensitivity) {
			Quaternion _desiredDirectionalInput = Quaternion.identity;
			_desiredDirectionalInput.eulerAngles = new Vector3(pitch, rigidbody.rotation.eulerAngles.y, bank);
			
			rigidbody.MoveRotation(Quaternion.Lerp( rigidbody.rotation, _desiredDirectionalInput, sensitivity * Time.deltaTime));
		}
		
		private void applyWindNoise() {
			
			if (!windNoiseClip)
				return;
			
			AudioSource windNoiseSource = soundManager.getSource (windNoiseClip);
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
					windNoiseSource.Play ();
			} else {
				windNoiseSource.Stop ();
			}
			
		}

		/// <summary>
		/// Calculates pitch, based on user input and configured pitch parameters.
		/// </summary>
		/// <returns>The pitch in degrees.</returns>
		/// <param name="flare">If set to <c>true</c> calculates pitch of a flare angle.</param>
		protected float getPitch(bool flare) {
			return flightInputs.inputPitch * maxPitch;
		}
		
		protected float getBank() {
			return flightInputs.inputBank * maxTurnBank;
		}
		
	
	}
}
