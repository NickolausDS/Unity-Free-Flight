using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	/// <summary>
	/// The flapping mechanic provides thrust and lift for flight objects. It supports multiple sounds, animation, and
	/// some general settings with regard to flap behavior. 
	/// </summary>
	[Serializable]
	public class Walking : Mechanic  {

		[Header("Inputs")]
		public string forwardAxis = "Vertical";
		public bool invertForward = true;
		public string turningAxis = "Horizontal";

		[Header("Animation Parameters")]
		[Tooltip("Animation Controller bool parameter for walking animation")]
		public string walkBool = "";
		private int walkingHash;
		[Tooltip("Animation Controller float parameter for walking speed")]
		public string walkSpeed = "";
		private int walkingSpeedHash;
		[Tooltip("Animation Controller float parameter for walking angular speed")]
		public string angularSpeed = "";
		private int angularSpeedHash;

		[Header("Sound")]
		public AudioClip[] sounds;
		public SoundManager soundManager = new SoundManager();

		[Header("General")]
		//meters/second
		public float maxGroundForwardSpeed = 40;
		//degrees/second
		public float groundDrag = 5;
		public float maxGroundTurningDegreesSecond = 40;

		public override void init (GameObject go, System.Object customPhysics) {
			base.init (go, customPhysics);
			soundManager.init (go);
			setupAnimation (walkBool, ref walkingHash);
			setupAnimation (walkSpeed, ref walkingSpeedHash);
			setupAnimation (angularSpeed, ref angularSpeedHash);
		}

		public override bool FFInputSatisfied () {
			return Input.GetAxis (forwardAxis) != 0 ? true : false;
		}

		/// <summary>
		/// Override FFStart to do nothing. The Stock Begin() isn't what we want
		/// </summary>
		public override void FFStart () {
		}

		public override void FFFixedUpdate () {
			rigidbody.drag = groundDrag;
			float inversion = (invertForward ? -1f : 1f);
			if (Input.GetAxis(forwardAxis) * inversion > 0f) {
				if (walkingHash != 0)
					animator.SetBool (walkingHash, true);
				rigidbody.AddRelativeForce (Vector3.forward * maxGroundForwardSpeed * Input.GetAxis(forwardAxis) * inversion * Time.deltaTime, ForceMode.VelocityChange);
			} else {
				if (walkingHash != 0)
					animator.SetBool (walkingHash, false);
			}
			
			float turningSpeed = maxGroundTurningDegreesSecond * Input.GetAxis (turningAxis) * Time.deltaTime;
			rigidbody.rotation *= Quaternion.AngleAxis (turningSpeed, Vector3.up);

			if (walkingSpeedHash != 0)
				animator.SetFloat (walkingSpeedHash, rigidbody.velocity.magnitude);
			if (angularSpeedHash != 0)
				animator.SetFloat (angularSpeedHash, turningSpeed);
		}

		/// <summary>
		/// Since flapping animation is done on a trigger, we want to override the default behavior.
		/// </summary>
		public override bool FFFinish () {
			if (walkingHash != 0)
				animator.SetBool (walkingHash, false);
			return true;
		}

		/// <summary>
		/// Launchs if airborn.
		/// </summary>
		/// <param name="minHeight">Minimum height.</param>
		private void launchIfAirborn(float minHeight) {
			if (!Physics.Raycast (rigidbody.position, Vector3.down, minHeight)) {
				gameObject.SendMessage ("SwitchModes", MovementModes.Flight, SendMessageOptions.RequireReceiver);
			}
		}


	}
}
