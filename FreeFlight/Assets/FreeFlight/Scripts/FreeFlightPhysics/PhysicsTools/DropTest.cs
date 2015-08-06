using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Physics testing for how a rigidbody moves an object over 
/// a duration of time and physics "ticks" (Fixed update calls)
/// </summary>
public class DropTest : MonoBehaviour {

	public float collisionTime = 0f;
	public bool collisionTimer = true;
	public Vector3 gravity = Physics.gravity;
	public float fixedUpdateTimeScale = .02f;
	public Vector3 antiForce = Vector3.zero;
	// Use this for initialization
	Rigidbody rb;
	private int startTime = -1;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		Physics.gravity = gravity;
		Time.fixedDeltaTime = fixedUpdateTimeScale;
	}

	void FixedUpdate() {
//		if (startTime == 0f) {
//			startTime = Time.realtimeSinceStartup;
//			Debug.Log ("Start time set at " + startTime);
//		}
		startTime++;

		Debug.Log (string.Format (
			"Fixed Update Ticks: {0}\n" +
			"Position: {1}\n" +
			"Velocity: {2}\n", startTime, rb.position, rb.velocity.magnitude));

		rb.AddForce (antiForce, ForceMode.Force);
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col) {
		collisionTime = Time.realtimeSinceStartup;
	}
}
