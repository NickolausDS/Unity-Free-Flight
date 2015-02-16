using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {
	
	[Serializable]
	public class Diving : Mechanic {

		public override void init (GameObject go, SoundManager sm, FreeFlightPhysics fp, FlightInputs fi) {
			base.init (go, sm, fp, fi);
			name = "Diving Mechanic";
			animationStateName = "Diving";
			animationStateHash = Animator.StringToHash (animationStateName);
		}
		
		public override bool FFInputSatisfied () {
			return flightInputs.inputDiving;
		}
		
		public override void FFFixedUpdate () {
			//Flare is the same as directional input, except with exagerated pitch and custom speed. 
			wingFold (flightInputs.inputLeftWingExposure, flightInputs.inputRightWingExposure);
		}
		
		
		public void wingFold(float left, float right) {
			flightPhysics.setWingPosition (left, right);
			
			float torqueSpeed = (rigidbody.velocity.magnitude) / 15.0f;
			rigidbody.AddTorque (rigidbody.rotation * Vector3.forward * (right - left) * torqueSpeed);
			rigidbody.angularDrag = left * right * 3.0f;
			
			if (!flightPhysics.wingsOpen()) {
				//Rotate the pitch down based on the angle of attack
				//This gives the player the feeling of falling
				Quaternion pitchRot = Quaternion.identity;
				pitchRot.eulerAngles = new Vector3 (flightPhysics.AngleOfAttack, 0, 0);
				pitchRot = rigidbody.rotation * pitchRot;
				rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, pitchRot, torqueSpeed * Time.deltaTime);
			}
			
		}
		
	}
}
