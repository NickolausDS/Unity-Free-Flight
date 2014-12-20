using UnityEngine;
using System.Collections;

/// <summary>
///Flight Mechanics is responsible for converting user input into 
///manipulating the wing for various physics effects.
/// 
///It handles the math behind flapping, flarring, and diving
///to change how physics effects the flight object. It does NOT
///directly interact with the user, ever. Instead, it's intended
///that the main free flight script pass in all user input, and
///call methods from here through Fixed Update()
/// </summary>
/// 
public class CreatureFlightPhysics : FlightPhysics {

	protected bool isFlapping = false;
	public bool IsFlapping { get { return isFlapping; } }
	private bool wingsHaveFlappedInDownPosition = false;
	protected float currentFlapTime = 0.0f;

	public void execute(CreatureFlightPhysics controller) {

	}

	public CreatureFlightPhysics (Rigidbody rb) : base(rb) {}


	public void directionalInput(float bank, float pitch, float sensitivity) {
		Quaternion _desiredDirectionalInput = Quaternion.identity;
		_desiredDirectionalInput.eulerAngles = new Vector3(pitch, rigidbody.rotation.eulerAngles.y, bank);
		
		rigidbody.MoveRotation(Quaternion.Lerp( rigidbody.rotation, _desiredDirectionalInput, sensitivity * Time.deltaTime));
	}
	
	/// <summary>
	/// Try to execute a flap. A regular flap can only happen in regular intervals
	/// determined by regFlapTime. A quickFlap is faster, and can happen in minFlapTime
	/// intervals or regular flaptime intervals. If we are not flapping, and the wings are
	/// open, we are guaranteed a flap. 
	/// </summary>
	/// <param name="minFlapTime">Minimum flap time.</param>
	/// <param name="regFlapTime">Reg flap time.</param>
	/// <param name="flapStrength">Flap strength.</param>
	/// <param name="downbeatStrength">Reverse of flap, when the wings are going back to normal position and we loose a little height. 
	/// This number is usually greator than real world values, as it gives the user the 'bobbing feeling of flight'. 
	/// <param name="regFlap">If set to <c>true</c> reg flap.</param>
	/// <param name="quickFlap">If set to <c>true</c> quick flap.</param>
	public void flap(float minFlapTime, float regFlapTime, float flapStrength, float downBeatStrength, bool regFlap, bool quickFlap) {
		currentFlapTime += Time.deltaTime;

		if (regFlap && wingsOpen ()) {
			//We can only flap if we're not currently flapping, or the user triggered
			//an 'interruptFlap', which just means "We're flapping faster than the regular
			//flap speed." InterruptFlaps are usually triggered by the user mashing the flap
			//button rather than just holding it down.
			if (!isFlapping || (quickFlap && currentFlapTime > minFlapTime)) {
				isFlapping = true;
				currentFlapTime = 0.0f;
				rigidbody.AddForce (rigidbody.rotation * Vector3.up * flapStrength);
			}
			
			//Here we deal with flapping at a regular interval. It may be better to use something
			//other than time.deltatime, it may give us incorrect readings
			if (isFlapping) {
				if (currentFlapTime > regFlapTime * 0.9f && !wingsHaveFlappedInDownPosition) {
					rigidbody.AddForce(rigidbody.rotation * Vector3.down * downBeatStrength);
					wingsHaveFlappedInDownPosition = true;
				} else if (currentFlapTime > regFlapTime) {
					isFlapping = false;
					wingsHaveFlappedInDownPosition = false;
				}
			}

		}
	}

//	public void wingFlare(float flareAngle, float flareSpeed) {
//		//Set wings fully open
//		setWingPosition (1.0f, 1.0f);
//
//		//Expose the true pitch, by rotating the Y value to zero
//		Quaternion rotation = Quaternion.LookRotation (new Vector3 (0.01f, -rigidbody.rotation.eulerAngles.y, 0));
//		rotation = rigidbody.rotation * rotation;
//
//		//Rotate to flare angle
//		if (rotation.eulerAngles.x > flareAngle) {
//			Quaternion desiredRotation = rigidbody.rotation * Quaternion.LookRotation(new Vector3(0, flareAngle, 0));
//			rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, desiredRotation, flareSpeed * Time.deltaTime);
//		}
//	}
//
//	public bool wingFlareRelease (float flareSpeed) {
//		rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, flareRotationSnapshot, flareSpeed * Time.deltaTime);
//		if (Quaternion.Angle (rigidbody.rotation, flareRotationSnapshot) < 5.0f) {
//			return true;
//		} else {
//			return false;
//		}
//	}

	public void wingFold(float left, float right) {
		setWingPosition (left, right);

		float torqueSpeed = (rigidbody.velocity.magnitude) / 15.0f;
		rigidbody.AddTorque (rigidbody.rotation * Vector3.forward * (right - left) * torqueSpeed);
		rigidbody.angularDrag = left * right * 3.0f;

		if (!wingsOpen()) {
			//Rotate the pitch down based on the angle of attack
			//This gives the player the feeling of falling
			Quaternion pitchRot = Quaternion.identity;
			pitchRot.eulerAngles = new Vector3 (AngleOfAttack, 0, 0);
			pitchRot = rigidbody.rotation * pitchRot;
			rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, pitchRot, torqueSpeed * Time.deltaTime);
		}

	}

	public void thrust(float forceNewtons) {
//		rigidbody.AddForce(new Vector3(0.0f, 0.0f, forceNewtons); 
	}


	//Get (in degrees) angle of attack for zero lift coefficient
	private float getAOAForZeroLCOF() {
		return 0.0f;
	}




}
