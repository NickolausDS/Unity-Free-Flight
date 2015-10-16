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

		[Header("Animation")]
		public string walkingAnimation = "Walking";
		private int walkingHash;
		public string walkingSpeed = "Speed";
		private int walkingSpeedHash;
		public string angularSpeed = "AngularSpeed";
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
			setupAnimation (walkingAnimation, ref walkingHash);
			setupAnimation (walkingSpeed, ref walkingSpeedHash);
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
				animator.SetBool (walkingHash, true);
				rigidbody.AddRelativeForce (Vector3.forward * maxGroundForwardSpeed * Input.GetAxis(forwardAxis) * inversion * Time.deltaTime, ForceMode.VelocityChange);
			} else {
				animator.SetBool (walkingHash, false);
			}
			
			float turningSpeed = maxGroundTurningDegreesSecond * Input.GetAxis (turningAxis) * Time.deltaTime;
			rigidbody.rotation *= Quaternion.AngleAxis (turningSpeed, Vector3.up);
			
			animator.SetFloat (walkingSpeedHash, rigidbody.velocity.magnitude);
			animator.SetFloat (angularSpeedHash, turningSpeed);
		}

		/// <summary>
		/// Since flapping animation is done on a trigger, we want to override the default behavior.
		/// </summary>
		public override bool FFFinish () {
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
