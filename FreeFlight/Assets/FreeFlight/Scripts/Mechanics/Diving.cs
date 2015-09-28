using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {
	
	[Serializable]
	public class Diving : Mechanic {

		[Header("Animation")]
		public string divingAnimation = "Diving";
		private int divingHash;

		[Header("Sound")]
		public AudioClip divingSound;
		public SoundManager soundManager = new SoundManager();

		private FlightPhysics flightPhysics;
		private FlightInputs flightInputs;

		public override void init (GameObject go, System.Object customPhysics, Inputs inputs) {
			flightPhysics = (FlightPhysics)customPhysics;
			flightInputs = (FlightInputs)inputs;
			base.init (go);
			name = "Diving Mechanic";
			setupAnimation (divingAnimation, ref divingHash);

		}
		
		public override bool FFInputSatisfied () {
			return flightInputs.inputDiving;
		}

		public override void FFStart () {
			animator.SetBool (divingHash, true);
		}
		
		public override void FFFixedUpdate () {
			//Flare is the same as directional input, except with exagerated pitch and custom speed. 
			wingFold (flightInputs.inputLeftWingExposure, flightInputs.inputRightWingExposure);
		}

		public override bool FFFinish ()
		{
			animator.SetBool (divingHash, false);
			return true;
		}
		
		
		public void wingFold(float left, float right) {
			flightPhysics.setWingExposure (left, right);
			
			float torqueSpeed = (rigidbody.velocity.magnitude) / 15.0f;
			rigidbody.AddTorque (rigidbody.rotation * Vector3.forward * (right - left) * torqueSpeed);
			rigidbody.angularDrag = left * right * 3.0f;
			
			if (!flightPhysics.wingsOpen()) {
				//Rotate the pitch down based on the angle of attack
				//This gives the player the feeling of falling
				Quaternion pitchRot = Quaternion.identity;
				pitchRot.eulerAngles = new Vector3 (flightPhysics.angleOfAttack, 0, 0);
				pitchRot = rigidbody.rotation * pitchRot;
				rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, pitchRot, torqueSpeed * Time.deltaTime);
			}
			
		}
		
	}
}
