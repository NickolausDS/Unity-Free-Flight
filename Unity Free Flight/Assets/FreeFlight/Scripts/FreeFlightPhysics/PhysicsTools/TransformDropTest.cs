using UnityEngine;
using System.Collections;

/// <summary>
/// This component is a reimplementation of rigidbody force and drag physics. 
/// It's useful for replicating rigidbody behaviour without using them. 
/// </summary>
public class TransformDropTest : MonoBehaviour {
	
	public float mass = 1f;
	public float drag;
	public Vector3 acceleration = Vector3.zero;
	public Vector3 gravity = Physics.gravity;

	private int fixedUpdateTicks = -1;
	public bool atRest = false;
	
	void FixedUpdate() {

		//Stop moving if we're at rest. 
		if (atRest)
			return;

		//Log debugging information
		fixedUpdateTicks++;
		Debug.Log (string.Format (
			"Fixed Update Ticks: {0}\n" +
			"Position: {1}\n" +
			//To derive velocity from acceleration, we need to divide by time
			"Velocity: {2}\n", fixedUpdateTicks, transform.position, acceleration.magnitude/Time.fixedDeltaTime));

		//Add the force
		AddForce (gravity, ForceMode.Acceleration);

		//Apply Linear Damping (drag)
		ApplyDrag ();

		//Move the object
		transform.position += acceleration;

	}

	void OnTriggerEnter() {
		atRest = true;
	}

	public void AddForce(Vector3 force, ForceMode forceType) {
		switch (forceType) {
		case ForceMode.Force:
			//Force mode moves an object according to it's mass
			acceleration = acceleration + force * mass * Time.fixedDeltaTime * Time.fixedDeltaTime;
			break;
		case ForceMode.Acceleration:
			//Acceleration ignores mass
			acceleration = acceleration + force * Time.fixedDeltaTime * Time.fixedDeltaTime;
			break;
		default:
			throw new UnityException("Force mode not supported!");
		}
	}

	//Apply Linear Damping
	public void ApplyDrag() {
		acceleration = acceleration * (1 - Time.deltaTime * drag);
	}
}
