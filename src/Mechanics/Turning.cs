using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	/// <summary>
	/// Do a banked turn based on player input. Allows setting up a banking parameter 
	/// for banked turns.
	/// </summary>
	[Serializable]
	public class Turning : Mechanic {

		[Header("Inputs")]
		public string axis = "Horizontal";
		public bool inverted = true;

		[Header("Animation")]
		[Tooltip("Amount of bank is in degrees")]
		public string bankingParameter = "";
		private int bankingHash;
		
		[Header("General")]
		//Basic gliding input, values in degrees
		public float maxBank = 45.0f;
		public float bankSensitivity = 2.0f;
		private float currentBank;
		
		private FlightPhysics flightPhysics;

		public override void init (GameObject go, System.Object customPhysics) {
			flightPhysics = (FlightPhysics)customPhysics;
			base.init (go);
			setupAnimation (bankingParameter, ref bankingHash);
		}
		
		public override bool FFInputSatisfied () {
			return (Input.GetAxis(axis) != 0f);
		}
		
		public override void FFStart () {}
		
		public override void FFFixedUpdate () {
			currentBank = Input.GetAxis (axis) * maxBank * -1f;
			if (bankingHash != 0)
				animator.SetFloat(bankingHash, currentBank);
			flightPhysics.addBank (currentBank, bankSensitivity);
		}
		
		public override bool FFFinish () {
			if (bankingHash != 0)
				animator.SetFloat (bankingHash, 0);
			return true;
		}
		
	}
}

