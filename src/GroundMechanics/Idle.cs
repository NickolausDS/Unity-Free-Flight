using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	/// <summary>
	/// Idle is a simple 'default state', and may play an animation or sound until 
	/// another mechanic plays or the mode is changed.
	/// </summary>
	[Serializable]
	public class Idle: Mechanic  {

		[Header("Animation Parameters")]
		[Tooltip("Animation Controller bool parameter for idle 'standing' animation")]
		public string idleBool;
		private int idleHash;

		[Header("Sound")]
		public AudioClip[] sounds;
		public SoundManager soundManager = new SoundManager();

		public override void init (GameObject go, System.Object customPhysics) {
			base.init (go, customPhysics);
			soundManager.init (go);
			name = "Idle Mechanic";
			setupAnimation (idleBool, ref idleHash);
		}

		public override bool FFInputSatisfied () {
			return true;
		}

		public override void FFStart () {
			if (idleHash != 0)
				animator.SetBool (idleHash, true);
		}

		public override bool FFFinish () {
			if (idleHash != 0)
				animator.SetBool (idleHash, false);
			return true;
		}

		
	}
}
