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
		private FlightInputs flightInputs;
		
		public override void init (GameObject go, System.Object customPhysics, Inputs inputs) {
			flightPhysics = (FlightPhysics)customPhysics;
			flightInputs = (FlightInputs)inputs;
			base.init (go);
			setupAnimation (bankingParameter, ref bankingHash);
		}
		
		public override bool FFInputSatisfied () {
			return (flightInputs.inputBank != 0f);
		}
		
		public override void FFStart () {}
		
		public override void FFFixedUpdate () {
			currentBank = flightInputs.inputBank * maxBank;
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

