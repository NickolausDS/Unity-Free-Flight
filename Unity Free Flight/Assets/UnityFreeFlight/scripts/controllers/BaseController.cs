using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour {

	protected float _rotationSpeed = 200.0f;
	protected Quaternion _userInput;
	protected int _invertedSetting = -1;


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

	// Use this for initialization
	void Start () {
	
	}


}
