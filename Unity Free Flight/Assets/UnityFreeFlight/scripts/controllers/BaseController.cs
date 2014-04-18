using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour {

	protected float _rotationSpeed = 200.0f;
	protected Quaternion _userInput;


	public Quaternion UserInput { get { return _userInput; } }

	// Use this for initialization
	void Start () {
	
	}


}
