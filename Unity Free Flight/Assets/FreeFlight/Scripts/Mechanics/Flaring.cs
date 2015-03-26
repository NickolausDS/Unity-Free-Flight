using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {
	
	[Serializable]
	public class Flaring : Mechanic {
		
		public bool enabledFlaring = false;
		public AudioClip flareSoundClip;
		//private AudioSource flareSoundSource;
		//The default pitch (x) we rotate to when we do a flare
		public float flareAngle = 70.0f;
		public float flareSpeed = 3.0f;
		public float pitchAdjustment = 20f;
		public float bankAdjustment = 35f;
		public float directionalSensitivity = 2.0f;

		
		
		public override void init (GameObject go, SoundManager sm, FlightPhysics fp, FlightInputs fi) {
			base.init (go, sm, fp, fi);
			name = "Flaring Mechanic";
			animationStateName = "Flaring";
			animationStateHash = Animator.StringToHash (animationStateName);
		}
		
		public override bool FFInputSatisfied () {
			return flightInputs.inputFlaring;
		}
		
		public override void FFFixedUpdate () {
			//Flare is the same as directional input, except with exagerated pitch and custom speed. 
			directionalInput(getBank (), getPitch (), directionalSensitivity);
		}


		public void directionalInput(float bank, float pitch, float sensitivity) {
			Quaternion _desiredDirectionalInput = Quaternion.identity;
			_desiredDirectionalInput.eulerAngles = new Vector3(pitch, rigidbody.rotation.eulerAngles.y, bank);
			rigidbody.MoveRotation (Quaternion.Lerp (rigidbody.rotation, _desiredDirectionalInput, sensitivity * Time.deltaTime));	
		}

		protected float getPitch() {
			return flightInputs.inputPitch * pitchAdjustment - flareAngle;
		}
		
		protected float getBank() {
			return flightInputs.inputBank * bankAdjustment;
		}
		
	}
}
