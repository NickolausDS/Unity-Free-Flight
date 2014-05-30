using UnityEngine;
using System.Collections;

public class KeyboardController : BaseFlightController {

	Vector3 keyInput;
	float rotationSpeed = 200.0f;

	public bool useGroundController = true;

	public float doubleJumpTimer = 0.7f;
	private float doubleJump;


	void Update() {

		//Don't allow any user flight controls when we're grounded. This lets
		//Ground controls take over so we don't interfere. 
		if (flightEnabled) {
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
		//The jump button re-enables flight-mode
		} else {
			if (Input.GetButtonDown("Jump") ) {
				if (doubleJump > 0.0f)
					enableFlight = true;
				else
					//Reset the timer
					doubleJump = doubleJumpTimer;
			}
		}

		doubleJump -= Time.deltaTime;

	}

	//Switch to a ground controller when we collide with the ground
	public void OnCollisionEnter(Collision col) {
		if (flightEnabled && useGroundController)
			enableGround = true;
	}
	


}
