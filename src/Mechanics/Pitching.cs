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

		[Header("Inputs")]
		public string axis = "Vertical";
		public bool inverted = true;

		[Header("Animation Parameters")]
		[Tooltip("Animation Controller float for amount of pitch in degrees")]
		public string pitchFloat = "";
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

		public override void init (GameObject go, System.Object customPhysics) {
			flightPhysics = (FlightPhysics)customPhysics;
			base.init (go);
			setupAnimation (pitchFloat, ref pitchHash);
		}

		/// <summary>
		/// Only possible if airspeed exceedes minimum airspeed and is lower than maximum angle of attack.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public override bool FFInputSatisfied () {
			return (Input.GetAxis(axis) != 0f && flightPhysics.angleOfAttack < maximumAngleOfAttack && flightPhysics.airspeed > minimumAirspeed);
		}
		
		public override void FFStart () {}
		
		public override void FFFixedUpdate () {
			currentPitch = Input.GetAxis(axis) * maxPitch * (inverted ? -1f : 1f);
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

