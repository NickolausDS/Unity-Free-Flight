using UnityEngine;
using System.Collections;

public class SimpleFlightController : BaseFlightController {

	Vector3 keyInput;

	public bool flappingEnabled = false;
	public float flaptime = 0.5f;
	public float flapStrength = 20.0f;

	//Private flapping vars
	private bool flapping;
	private bool wingsHaveFlappedInDownPosition;
	private float currentFlaptime;

	void Update() {
		//Pitch
		keyInput.x = _invertedSetting * -Input.GetAxis ("Vertical") * (_rotationSpeed * Time.deltaTime);
		//Roll
		keyInput.z = -Input.GetAxis ("Horizontal") * (_rotationSpeed * Time.deltaTime);
		//Yaw
		//	keyInput.y = Input.GetAxis("Yaw") * (_rotationSpeed * Time.deltaTime);
		_userInput.eulerAngles = keyInput;

		if (flappingEnabled) {
			flapWings ();
		}

	}

	void flapWings() {

		if (flapping) {
			currentFlaptime += Time.deltaTime;
			if (currentFlaptime > flaptime * 0.9f && !wingsHaveFlappedInDownPosition) {
				rigidbody.AddForce( new Vector3 (0, -flapStrength/2, 0));
				wingsHaveFlappedInDownPosition = true;
			} else if (currentFlaptime > flaptime) {
				flapping = false;
				wingsHaveFlappedInDownPosition = false;
			}
		} else if (Input.GetButton("Jump") && !flapping) {
			flapping = true;
			currentFlaptime = 0.0f;
			rigidbody.AddForce (new Vector3 (0, flapStrength, 0));
		}
	}


}
