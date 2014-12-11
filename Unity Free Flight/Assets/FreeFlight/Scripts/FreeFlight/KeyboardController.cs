using UnityEngine;
using System.Collections;

public class KeyboardController : BaseFlightController {
	
	public float launchTime = 0.2f;
	private float launchTimeTimer;


	void Update() {
		//Don't allow any user flight controls when we're grounded. This lets
		//Ground controls take over so we don't interfere. 
		if (isFlying) {
			_inputPitch = _inputInvertedSetting * -Input.GetAxis("Vertical");
			_inputBank = -Input.GetAxis ("Horizontal");

			if (enabledFlaring)
				_inputFlaring = Input.GetButton("WingFlare");

			//If the user presses down the jump button, flap
			_inputFlap = Input.GetButton("Jump"); 


			if (enabledDiving) {
				if(Input.GetButton ("FoldLeftWing"))
					_inputLeftWingExposure = 0.0f;
				else 
					_inputLeftWingExposure = 1.0f;

				if(Input.GetButton ("FoldRightWing")) 
					_inputRightWingExposure = 0.0f;
				else 
					_inputRightWingExposure = 1.0f;

				if (_inputLeftWingExposure < 1.0f || _inputRightWingExposure < 1.0f)
					_inputDiving = true;
				else {
					_inputDiving = false;
				}
			}
		//The jump button re-enables flight-mode
		} else {

			jumpLaunch(Input.GetButton("Jump"));

		}

	}

	void jumpLaunch(bool jumpState) {
		if (jumpState == true) {
			if (launchTimeTimer > launchTime)
				_inputTakeoff = true;
			else
				launchTimeTimer += Time.deltaTime;
		} else {
			launchTimeTimer = 0.0f;
			_inputTakeoff = false;
		}
	}

}
