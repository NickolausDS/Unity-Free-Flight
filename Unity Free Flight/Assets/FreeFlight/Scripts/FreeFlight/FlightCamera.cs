/// <summary>
/// flight camera.
/// 
/// flight camera handles camera movement and position for flight objects. Specifically, its 
/// important because it understands how to rotate during flight maneuvers, otherwise it is 
/// just another simple camera. 
/// </summary>
using UnityEngine;
using System.Collections;

public class FlightCamera : MonoBehaviour {

	public GameObject cam;
	public GameObject target;
	public bool thirdPersonMode = false;

	public Vector3 firstPersonPosition = new Vector3(0f,0f,.5f);
	public Vector3 thirdPersonPosition = new Vector3(0f,0.1f,-3f);

	public float thirdPersonLag = 2f;
	public float rotationSpeed = 1.0f;

	public float flareLookDuration = 2.0f;
	public float flareLookAngle = 50f;
	private float flareLookTimer = 0.0f;


	private BaseFlightController fc;

	// Use this for initialization
	void Awake () {
		if (!cam)
			cam = GameObject.FindGameObjectWithTag ("MainCamera");	
		if (!target)
			target = GameObject.FindGameObjectWithTag ("Player");
		//If we can't find the player by tag, assume we're already attached to the flight object.
		if (!target) {
			target = gameObject;
		}
		fc = target.GetComponent<BaseFlightController> ();
		if (!fc) {
			Debug.LogError (gameObject + ": Flight Camera: This script was designed to work with free flight objects, please set the 'target' to one.");
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (thirdPersonMode)
			iTween.MoveUpdate (cam, target.transform.TransformPoint (thirdPersonPosition), thirdPersonLag);
		else
			iTween.MoveUpdate (cam, target.transform.TransformPoint (firstPersonPosition), .1f);

		if (fc.InputFlaring || flareLookTimer > 0f) {
			flareLook ();
		} else {
			iTween.RotateUpdate (cam, target.transform.rotation.eulerAngles, rotationSpeed);
		}

	}

	/// <summary>
	/// Adjust the camera angle for a flare maneuver. 
	/// 
	/// The look direction will be the area the player wants to land, forward/below
	/// them about 50 degrees. 
	/// 
	/// You can adjust the look angle and duration by modifying the flarelook angle/timer
	/// variables above. 
	/// </summary>
	void flareLook () {
		if (fc.InputFlaring)
			flareLookTimer = flareLookDuration;
		else
			flareLookTimer -= Time.deltaTime;

		//Current pitch rotation from level axis
		float currentPitch = Mathf.DeltaAngle(target.transform.eulerAngles.x, 0);
		//Desired pitch we want the camera to look.
		float desiredpitch = Mathf.Abs (flareLookAngle + currentPitch);
		//reduce the amount of flare slowly once the player releases the button
		desiredpitch = desiredpitch * flareLookTimer / flareLookDuration;

		//Find the direction we want the camera to look, based on targets world rotation
		Quaternion dir = Quaternion.AngleAxis (desiredpitch, Vector3.right);
		dir = target.transform.rotation * dir;

		iTween.RotateUpdate (cam, dir.eulerAngles, rotationSpeed * 5.0f);


	}

	/// <summary>
	/// Find out if there are objects between the camera and the player object.
	/// </summary>
	/// <returns>The with objects.</returns>
//	float collidingWithObjects () {
//		RaycastHit hit;
//		int mask = 1 << LayerMask.NameToLayer ("Player");
//		mask = ~mask;
//
//
//		if (Physics.Raycast (cam.transform.position, cam.transform.forward, out hit, followDistance, mask)) {
//			return hit.distance;
//		}
//		return followDistance;
//
//	}

	
}
