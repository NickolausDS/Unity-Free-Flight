using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;


namespace UnityFreeFlight {

	/// <summary>
	/// Ground Mode inputs
	/// </summary>
	[Serializable]
	public class GroundInputs : Inputs {
	
		[Range(-1.0f, 1.0f)]
		protected float _inputGroundForward;
		[Range(-1.0f, 1.0f)]
		protected float _inputGroundTurning;
		protected bool _inputJump;
		protected bool _inputTakeoff = false;

		public bool inputTakeoff { get { return _inputTakeoff; } }
		public float inputGroundForward { get { return _inputGroundForward; } }
		public float inputGroundTurning { get { return _inputGroundTurning; } }
		public bool inputJump { get { return _inputJump; } }

		public override void getInputs() {
			_inputGroundForward = Input.GetAxis("Vertical");
			_inputGroundTurning = Input.GetAxis ("Horizontal");
			_inputJump = Input.GetButton ("Jump");
			_inputTakeoff = _inputJump;
		}

		public override void resetInputs () {
			_inputGroundForward = 0f;
			_inputGroundTurning = 0f;
			_inputJump = false;
			_inputTakeoff = false;
		}
	}
}