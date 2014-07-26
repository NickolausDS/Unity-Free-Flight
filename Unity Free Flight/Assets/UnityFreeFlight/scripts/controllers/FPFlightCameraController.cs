/// <summary>
/// Controls the camera in first person flight.
/// </summary>

using UnityEngine;
using System.Collections;

public class FPFlightCameraController : MonoBehaviour {

	public Transform cameraTransform;
	public BaseFlightController flightController;
	public float cameraSpeed = 3.0f;

	private Quaternion savedRotation;

	private bool alignedWithGameObject;
	// Use this for initialization
	void Awake () {
		if (!cameraTransform && Camera.main)
			cameraTransform = Camera.main.transform;
		if (!cameraTransform) {
			Debug.Log ("Please add a camera to FPFlightCameraController");
			enabled = false;
		}
		if (!flightController)
			flightController = gameObject.GetComponent<BaseFlightController>();
		if (!flightController) {
			Debug.Log ("Object does not have a flight controller. Please add one for FPFlightCameraController to work");
			enabled = false;
		}


	}
	
	// Update is called once per frame
	void Update () {

		if (flightController.flightEnabled) {

			if(flightController.IsFlaring) {
				cameraTransform.rotation = savedRotation;
				alignedWithGameObject = false;
				//cameraTransform.rotation *= Quaternion.LookRotation( new Vector3 ( 0, -flightController.FlareAngle, 0) );
			} else {
				if (!alignedWithGameObject)
					alignWithGameObject();
				cameraTransform.rotation = Quaternion.Lerp (cameraTransform.rotation, gameObject.transform.rotation, cameraSpeed * Time.deltaTime );
				savedRotation = cameraTransform.rotation;
			}
		} else {
			if (!alignedWithGameObject)
				alignWithGameObject();
		}
	
	}

	void alignWithGameObject() {
		cameraTransform.rotation = Quaternion.Lerp (cameraTransform.rotation, gameObject.transform.rotation, cameraSpeed * Time.deltaTime );
		if (Quaternion.Angle (cameraTransform.rotation, gameObject.transform.rotation) < 0.5f)
			alignedWithGameObject = true;
	}
}
