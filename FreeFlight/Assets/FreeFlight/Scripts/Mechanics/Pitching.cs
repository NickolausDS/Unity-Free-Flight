using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {
	
	/// <summary>
	/// Pitch the player up or down based on player input
	/// </summary>
	[Serializable]
	public class Pitching : Mechanic {
		
		[Header("Animation")]
		[Tooltip("Amount of pitch is in degrees")]
		public string pitchParameter = "";
		private int pitchHash;
		
		[Header("General")]
		[Tooltip("The degrees of pitch that will be applied to the object (both positive and negative).")]
		public float maxPitch = 20.0f;
		[Tooltip("Speed pitch will be applied")]
		public float sensitivity = 2.0f;
		[Tooltip("Minimum airspeed needed before this mechanic can be applied.")]
		public float minimumAirspeed = 2f;
		[Tooltip("Maximum angle of attack. This value determines when an object 'stalls'")]
		public float maximumAngleOfAttack = 10f;
		private float currentPitch;
		
		private FlightPhysics flightPhysics;
		private FlightInputs flightInputs;
		
		public override void init (GameObject go, System.Object customPhysics, Inputs inputs) {
			flightPhysics = (FlightPhysics)customPhysics;
			flightInputs = (FlightInputs)inputs;
			base.init (go);
			setupAnimation (pitchParameter, ref pitchHash);
		}

		/// <summary>
		/// Only possible if airspeed exceedes minimum airspeed and is lower than maximum angle of attack.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public override bool FFInputSatisfied () {
			return (flightInputs.inputPitch != 0f && flightPhysics.angleOfAttack < maximumAngleOfAttack && flightPhysics.airspeed > minimumAirspeed);
		}
		
		public override void FFStart () {}
		
		public override void FFFixedUpdate () {
			currentPitch = flightInputs.inputPitch * maxPitch;
			if (pitchHash != 0)
				animator.SetFloat(pitchHash, currentPitch);
			flightPhysics.addPitch (currentPitch, sensitivity);
		}
		
		public override bool FFFinish () {
			if (pitchHash != 0)
				animator.SetFloat (pitchHash, 0);
			return true;
		}
		
	}
}

