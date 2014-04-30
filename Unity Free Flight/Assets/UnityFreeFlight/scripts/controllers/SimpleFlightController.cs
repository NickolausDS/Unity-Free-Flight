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
		} else if (Input.GetButton ("Jump")) {
			flapWings ();
		}

	}


}
