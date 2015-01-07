/// <summary>
/// flight camera.
/// 
/// flight camera handles camera movement and position for flight objects. Specifically, its 
/// important because it understands how to rotate during flight maneuvers, otherwise it is 
/// just another simple camera. 
/// </summary>
using UnityEngine;
using System.Collections;

[AddComponentMenu ("Scripts/Camera-Control/Flight Camera")]
public class FlightCamera : MonoBehaviour {

	public GameObject cam;
	public GameObject target;


	[Header("Regular Positioning")]
	public bool thirdPersonMode = false;

	public Vector3 firstPersonPosition = new Vector3(0f,0f,.5f);
	public Vector3 thirdPersonPosition = new Vector3(0f,0.1f,-2.5f);

	public float thirdPersonLag = 1.8f;
	public float rotationLag = 1.0f;

	//Distance 3 height 3 Angle 50 works well for overhead view, useful for maximum view of the
	//terrain around the player, which they might like to do at high altitude
	//Distance -3 height 2 and zero angle works for low level perspective view,
	//which gives the player an understanding of how much height they have left on landing
	[Header("Flare Positioning")]
	public float flareCamHeight = 2f;
	public float flareCamDist = -3f;

	public float flareCamAngle = 50f;
	public float flareDuration = 1.0f;
	public float flareLag = .3f;


	private float flareLookTimer = 0.0f;
	private FreeFlight ff;

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
		ff = target.GetComponent<FreeFlight> ();
		if (!ff) {
			Debug.LogError (gameObject + ": Flight Camera: This script was designed to work with free flight objects, please set the 'target' to one.");
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (ff.InputFlaring || flareLookTimer > 0f) {
			flareLook ();
		} else {
			if (thirdPersonMode)
				iTween.MoveUpdate (cam, target.transform.TransformPoint (thirdPersonPosition), thirdPersonLag);
			else
				iTween.MoveUpdate (cam, target.transform.TransformPoint (firstPersonPosition), .1f);

			iTween.RotateUpdate (cam, target.transform.rotation.eulerAngles, rotationLag);
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
		if (!ff.isFlying()) {
			flareLookTimer = -1;
			return;
		}

		Vector3 flareLookPosition = new Vector3 (0f, flareCamHeight, flareCamDist);
		//Current pitch rotation from level axis
		float currentPitch = Mathf.DeltaAngle(target.transform.eulerAngles.x, 0);
		//Desired pitch we want the camera to look.
		float desiredpitch = Mathf.Abs (flareCamAngle + currentPitch);

		//Set or decrease the timer for doing a flare look
		if (ff.InputFlaring)
			flareLookTimer = flareDuration;
		else
			flareLookTimer -= Time.deltaTime;

		//reduce the amount of flare slowly once the player releases the button
		desiredpitch = desiredpitch * flareLookTimer / flareDuration;
		//Find the direction we want the camera to look, based on targets world rotation
		Quaternion dir = Quaternion.AngleAxis (desiredpitch, Vector3.right);
		dir = target.transform.rotation * dir;

		if (thirdPersonMode) {
			iTween.MoveUpdate (cam, target.transform.TransformPoint (flareLookPosition), flareLag);
			iTween.RotateUpdate (cam, dir.eulerAngles, rotationLag);
		} else { 
			iTween.MoveUpdate (cam, target.transform.TransformPoint (firstPersonPosition), flareLag);
			iTween.RotateUpdate (cam, dir.eulerAngles, rotationLag * 5.0f);
		}

	}

//HACK: This feature was never fully developed, and it is still undecided whether or not 
// it is actually needed. Something should happen to it by v0.5.x, or it should be removed.
	/// <summary>
	/// Find out if there are objects between the camera and the player object. If there are, reduce the follow
	/// distance so there is a clear view of the player
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
