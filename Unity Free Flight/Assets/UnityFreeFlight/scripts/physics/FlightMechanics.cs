using UnityEngine;
using System.Collections;

/*
 * Flight Mechanics is responsible for converting user input into 
 * manipulating the wing for various physics effects.
 * 
 * It handles the math behind flapping, flarring, diving,
 * to change how physics effects the flight object.
 * 
 * Example: In order to do a 'dive', this class reduces the
 * wing area. flight physics then calculates smaller lift forces,
 * allowing the flight object to plummet.
 */ 

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

	protected bool isFlapping = 0.0f;
	protected float currentFlapTime = 0.0f;

	public FlightMechanics(Rigidbody rb) : base(rb) {

	}


	public void flap(float flapStrength) {
		if (wingsOpen ()) {
			//do stuff
			return;
		}
	}


	public void wingFlare() {
		return;
	}

	public void wingFold(float left, float right) {
		setWingPosition (left, right);

	}

	public void thrust(float forceNewtons) {
//		rigidbody.AddForce(new Vector3(0.0f, 0.0f, forceNewtons); 
	}


	//Get (in degrees) angle of attack for zero lift coefficient
	private float getAOAForZeroLCOF() {
		return 0.0f;
	}




}
