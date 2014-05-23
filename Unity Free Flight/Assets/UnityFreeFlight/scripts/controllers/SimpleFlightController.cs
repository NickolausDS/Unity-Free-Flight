using UnityEngine;
using System.Collections;

public class SimpleFlightController : BaseFlightController {

	Vector3 keyInput;
	float rotationSpeed = 200.0f;


	void Update() {
		//Pitch
		keyInput.x = _invertedSetting * -Input.GetAxis ("Vertical") * rotationSpeed * Time.deltaTime;
		//Roll
		keyInput.z = -Input.GetAxis ("Horizontal") * (rotationSpeed * Time.deltaTime);
		_userInput.eulerAngles = keyInput;

		if (Input.GetButtonDown("Jump") ) {
			flapWings (true);
			flightEnabled = true;
		} else if (Input.GetButton ("Jump")) {
			flapWings ();
		}

		if (Input.GetButton("FoldLeftWing")) {
			_leftWingExposure = 0.0f;
		} else {
			_leftWingExposure = 1.0f;
		}

		if (Input.GetButton("FoldRightWing")) {
			_rightWingExposure = 0.0f;
		} else {
			_rightWingExposure = 1.0f;
		}






	}


}
