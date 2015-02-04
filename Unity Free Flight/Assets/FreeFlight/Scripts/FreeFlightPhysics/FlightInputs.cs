using UnityEngine;
using System.Collections;


namespace UnityFreeFlight {

	/// <summary>
	/// Hold inputs from the player. Note, that this hasn't been properly refactored since version 0.4.x
	/// </summary>
	public class FlightInputs : Inputs {

		//These protected vars are meant to be directly used or modified by the 
		//child class, and generally read from by the physics model. 
		[Range(0.0f, 1.0f)]
		protected float _inputLeftWingExposure = 1.0f;
		[Range(0.0f, 1.0f)]
		protected float _inputRightWingExposure = 1.0f;
		protected int _inputInvertedSetting = -1;
		protected bool _inputFlaring = false;
		protected bool _inputDiving = false;
		protected bool _inputFlap = false;
		[Range(-1.0f, 1.0f)]
		protected float _inputPitch = 0.0f;
		[Range(-1.0f, 1.0f)]
		protected float _inputBank = 0.0f;

		public float inputLeftWingExposure { get {return _inputLeftWingExposure; } }
		public float inputRightWingExposure { get {return _inputRightWingExposure; } }
		public bool inputFlaring { get { return _inputFlaring; } }
		public bool inputDiving { get { return _inputDiving; } }
		public bool inputFlap { get { return _inputFlap; } }
		public float inputPitch { get { return _inputPitch; } }
//		public float anglePitch { get { return getPitch(_inputFlaring); } }
		public float inputBank { get { return _inputBank; } }
//		public float angleBank { get { return getBank (); } }

		//Even though Inverted as a property here is invisible to the inspector, 
		//using the property in this way makes it convienient to access externally,
		//in order to *toggle* the setting on and off. Expressing _invertedSetting internally
		//an integer makes it very easy to apply to input.
		public bool Inverted {
			get {
				if (_inputInvertedSetting == 1) 
					return true; 
				return false;
			}
			set {
				if (value == true) 
					_inputInvertedSetting = -1;
				else
					_inputInvertedSetting = 1;
			}
		}

		public override void resetInputs() {
			_inputLeftWingExposure = 1.0f;
			_inputRightWingExposure = 1.0f;
			_inputFlaring = false;
			_inputDiving = false;
			_inputFlap = false;
			_inputPitch = 0.0f;
			_inputBank = 0.0f;
		}

		public override void getInputs() { 
			_inputPitch = _inputInvertedSetting * -Input.GetAxis("Vertical");
			_inputBank = -Input.GetAxis ("Horizontal");
			
//			if (enabledFlaring)
				_inputFlaring = Input.GetButton("WingFlare");
			
			//If the user presses down the jump button, flap
			_inputFlap = Input.GetButton("Jump"); 
			
			
//			if (enabledDiving) {
				_inputDiving = Input.GetButton ("Dive");
				if (_inputDiving) {
					_inputLeftWingExposure = 0.0f;
					_inputRightWingExposure = 0.0f;
				} else {
					_inputRightWingExposure = 1.0f;
					_inputRightWingExposure = 1.0f;
//				}
			}
		}
	}
}
