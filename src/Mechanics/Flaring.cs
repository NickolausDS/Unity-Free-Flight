using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	/// <summary>
	/// Apply rapid pitch to slow down rapidly. 
	/// 
	/// NOTE: Currently applied pitch is a set amount, such as 70 degrees. Because this set amount does not
	/// account for speed or angle of attack, the outcome can be somewhat random. If the player is flying slow,
	/// they will drop out of the sky. If they are flying fast, they will be vaulted skyward. 
	/// </summary>
	[Serializable]
	public class Flaring : Mechanic {

		[Header("Inputs")]
		public string button = "WingFlare";

		[Header("Animation Parameters")]
		[Tooltip("Animation Controller bool for flaring animation")]
		public string flaringBool = "";
		private int flaringHash;

		[Header("Sound")]
		public AudioClip flareSoundClip;
		public SoundManager soundManager = new SoundManager();

		[Header("General")]
		//The default pitch (x) we rotate to when we do a flare
		[Range (0f, 90f)][Tooltip ("Amount of backwards pitch will be applied to slow down the object.")]
		public float flareAngle = 70.0f;
		public float flareSpeed = 3.0f;
		[Range (0f, 100f)][Tooltip ("Percentage of flare is visual rotation. Zero means rotation will affect physics but " +
			"will not appear visually, which is useful if you want the flare to be represented by animation instead of rotating the gameobject.")]
		public float rotationPercentage = 20f;
		
		private FlightPhysics flightPhysics;

		public override void init (GameObject go, System.Object customPhysics) {
			flightPhysics = (FlightPhysics)customPhysics;
			base.init (go);
			name = "Flaring Mechanic";
			setupAnimation (flaringBool, ref flaringHash);
		}
		
		public override bool FFInputSatisfied () {
			return Input.GetButton(button);
		}

		public override void FFStart () {
			animator.SetBool (flaringHash, true);
			soundManager.playSound (flareSoundClip);
		} 
		
		public override void FFFixedUpdate () {
			flightPhysics.addPhysicsPitch (flareAngle - (flareAngle * rotationPercentage / 100), flareSpeed, this);
			flightPhysics.addPitch(flareAngle * rotationPercentage / 100, flareSpeed);
		}

		public override bool FFFinish () {
			animator.SetBool (flaringHash, false);
			flightPhysics.releasePhysicsRotation (this);
			return true;
		}
		
	}
}
