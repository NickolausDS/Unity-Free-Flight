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
public class FlightMechanics : FlightPhysics {

	protected bool isFlapping = false;
	private bool wingsHaveFlappedInDownPosition = false;
	protected float currentFlapTime = 0.0f;

	protected Quaternion flareRotationSnapshot;
		
	public void execute(BaseFlightController controller) {

		//Find out how much our user turned us
		rigidbody.rotation *= controller.UserInput;

		base.doStandardPhysics ();

		if (controller.StartFlare) {
//			Debug.Log ("Starting Flare");
			flareRotationSnapshot = rigidbody.rotation;
			wingFlare(controller.FlareAngle, controller.flareSpeed);
		} else if (controller.IsFlaring && !controller.ReleaseFlare) {
//			Debug.Log ("Is Flaring");
			wingFlare (controller.FlareAngle, controller.flareSpeed);
		} else if (controller.IsFlaring && controller.ReleaseFlare) {
//			Debug.Log ("Releasing Flare");
			if (wingFlareRelease(controller.flareSpeed))
				controller.terminateFlare();
		} else {
			wingFold (controller.LeftWingExposure, controller.RightWingExposure);
		}

		flap (
			controller.minimumFlapTime,
			controller.regularFlaptime,
			controller.flapStrength,
			controller.downbeatStrength,
			controller.RegularFlap,
			controller.QuickFlap
		);



	}

	public FlightMechanics(Rigidbody rb) : base(rb) {}

	
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
			//do stuff
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

	public void wingFlare(float flareAngle, float flareSpeed) {
		//Set wings fully open
		setWingPosition (1.0f, 1.0f);

		//Expose the true pitch, by rotating the Y value to zero
		Quaternion rotation = Quaternion.LookRotation (new Vector3 (0.01f, -rigidbody.rotation.eulerAngles.y, 0));
		rotation = rigidbody.rotation * rotation;

		//Rotate to flare angle
		if (rotation.eulerAngles.x > flareAngle) {
			Quaternion desiredRotation = rigidbody.rotation * Quaternion.LookRotation(new Vector3(0, flareAngle, 0));
			rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, desiredRotation, flareSpeed * Time.deltaTime);
		}
	}

	public bool wingFlareRelease (float flareSpeed) {
		rigidbody.rotation = Quaternion.Lerp (rigidbody.rotation, flareRotationSnapshot, flareSpeed * Time.deltaTime);
		if (Quaternion.Angle (rigidbody.rotation, flareRotationSnapshot) < 5.0f) {
			return true;
		} else {
			return false;
		}
	}

	public void wingFold(float left, float right) {
		setWingPosition (left, right);
		rigidbody.AddTorque (rigidbody.rotation * Vector3.forward * (right - left) * 2.0f);
		rigidbody.angularDrag = left * right * 3.0f;

	}

	public void thrust(float forceNewtons) {
//		rigidbody.AddForce(new Vector3(0.0f, 0.0f, forceNewtons); 
	}


	//Get (in degrees) angle of attack for zero lift coefficient
	private float getAOAForZeroLCOF() {
		return 0.0f;
	}




}
