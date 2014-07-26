using UnityEngine;
using System.Collections;

public class MouseController : BaseFlightController {

	Vector3 keyInput;
	float rotationSpeed = 200.0f;
	
	public float launchTime = 0.2f;
	private float launchTimeTimer;


	void Update() {

		//Don't allow any user flight controls when we're grounded. This lets
		//Ground controls take over so we don't interfere. 
		if (flightEnabled) {
			//Pitch
			keyInput.x = _invertedSetting * -Input.GetAxis ("Mouse Y") * rotationSpeed * Time.deltaTime;
			//Roll
			keyInput.z = -Input.GetAxis ("Mouse X") * (rotationSpeed * Time.deltaTime);
			_userInput.eulerAngles = keyInput;

			if (Input.GetButtonDown("Jump") ) {
				flapWings (true);
			} else if (Input.GetButton ("Jump")) {
				flapWings ();
			}
			if (divingEnabled) {
				dive (Input.GetButton ("FoldLeftWing"), Input.GetButton("FoldRightWing"));
			}
			if (flaringEnabled && Input.GetButton ("WingFlare")) {
				doFlare = true;
			} else {
				doFlare = false;
			}
		//The jump button re-enables flight-mode
		} else {
			if (Input.GetButton("Jump") ) {
				if (launchTimeTimer > launchTime)
					enableFlight = true;
				else
					launchTimeTimer += Time.deltaTime;
			} else {
				launchTimeTimer = 0.0f;
			}
		}


	}



}
