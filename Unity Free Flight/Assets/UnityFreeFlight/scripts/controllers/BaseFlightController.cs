using UnityEngine;
using System.Collections;

public class BaseFlightController : MonoBehaviour {

	protected float _rotationSpeed = 200.0f;
	protected Quaternion _userInput;
	protected int _invertedSetting = -1;

	private bool _hasWarnedUser = false;
	public bool flightEnabled = true;

	public Quaternion UserInput { get { return _userInput; } }


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

	// Use this for initialization
	void Start () {
	
	}


}
