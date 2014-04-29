using UnityEngine;
using System.Collections;

public class BaseFlightController : MonoBehaviour {

	protected float _rotationSpeed = 200.0f;
	protected Quaternion _userInput;
	protected int _invertedSetting = -1;

	private bool _hasWarnedUser = false;
	public bool flightEnabled = true;

	public Quaternion UserInput { get { return _userInput; } }


	public bool flappingEnabled = false;
	public float regularFlaptime = 0.7f;
	public float minimumFlapTime = 0.2f;
	public float flapStrength = 400.0f;
	
	//Private flapping vars
	private bool flapping;
	private bool wingsHaveFlappedInDownPosition;
	private float currentFlaptime;

	public bool Inverted {
		get {
			if (_invertedSetting == 1) 
				return true; 
			return false;
		}
		set {
			if (value == true) 
				_invertedSetting = -1;
			else
				_invertedSetting = 1;
		}
	}
	

	void Update() {
		if (!_hasWarnedUser) {
			_hasWarnedUser = true;
			Debug.LogWarning ("Warning! No user controller added! Please drag a flight controller to: " + gameObject.name);
//			MonoBehaviour thisobj = gameObject.GetComponent<BaseFlightController>();
//			thisobj.SendMessage("setDefaultController", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void flapWings(bool interruptFlap = false) {

		if (flappingEnabled) {

			//We can only flap if we're not currently flapping, or the user triggered
			//an 'interruptFlap', which just means "We're flapping faster than the regular
			//flap speed." InterruptFlaps are usually triggered by the user mashing the flap
			//button rather than just holding it down.
			if (!flapping || (interruptFlap && currentFlaptime > minimumFlapTime)) {
				flapping = true;
				currentFlaptime = 0.0f;
				rigidbody.AddForce (new Vector3 (0, flapStrength, 0));
			}

			//Here we deal with flapping at a regular interval. It may be better to use something
			//other than time.deltatime, it may give us incorrect readings
			if (flapping) {
				currentFlaptime += Time.deltaTime;
				if (currentFlaptime > regularFlaptime * 0.9f && !wingsHaveFlappedInDownPosition) {
					rigidbody.AddForce( new Vector3 (0, -flapStrength/4, 0));
					wingsHaveFlappedInDownPosition = true;
				} else if (currentFlaptime > regularFlaptime) {
					flapping = false;
					wingsHaveFlappedInDownPosition = false;
				}
			}
		}

	}

	// Use this for initialization
	void Start () {
	
	}


}
