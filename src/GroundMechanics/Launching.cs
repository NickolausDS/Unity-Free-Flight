using UnityEngine;
using System.Collections;
using System;
using UnityFreeFlight;

namespace UnityFreeFlight {
	[Serializable]
	public class Launching : Mechanic {

		[Header("Inputs")]
		public string button = "Jump";

		[Header("Animation")]
		public string launchingAnimation;
		private int launchingHash;
		
		[Header("Sound")]
		public AudioClip[] sounds;
		public SoundManager soundManager = new SoundManager();
		
		[Header("General")]
		public float launchTime = 0.2f;
		private float launchTimer;
		public bool enabledLaunchIfAirborn = true;
		public float minHeightToLaunchIfAirborn = 2f;

		public override void init (GameObject go, System.Object customPhysics) {
			base.init (go, customPhysics);
			soundManager.init (go);
			setupAnimation (launchingAnimation, ref launchingHash);
		}

		public override bool FFInputSatisfied () {  
			return checkLaunchTimer(Input.GetButton(button));
		}

		public override void FFFixedUpdate () {
			gameObject.SendMessage ("SwitchModes", MovementModes.Flight, SendMessageOptions.RequireReceiver);
		}

		/// <summary>
		/// Starts takeoff after "triggerSet" has been true for "launchTime". 
		/// This method needs to be called in Update or FixedUpdate to work properly. 
		/// </summary>
		/// <param name="triggerSet">If set to <c>true</c> for duration of launchTimer, triggers takeoff.</param>
		private bool checkLaunchTimer(bool triggerSet) {
			if (triggerSet == true) {
				if (launchTimer > launchTime) {
					launchTimer = 0.0f;
					return true;
				} else {
					launchTimer += Time.deltaTime;
					return false;
				}
			} else {
				launchTimer = 0.0f;
				return false;
			}
		}
	}
}
