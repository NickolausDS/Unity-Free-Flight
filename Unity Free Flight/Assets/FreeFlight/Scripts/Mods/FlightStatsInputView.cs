using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using UnityFreeFlight;

public class FlightStatsInputView : MonoBehaviour {

	public GameObject flightObject;
	public Text text;
	private FreeFlight ffComponent;
	private FlightInputs fInputs;

	// Use this for initialization
	void Start () {
		autoConfig ();
		nullCheck ("", flightObject);
		nullCheck ("text", text);

	}
	
	void Update () {
		updateText ();
	}

	public void updateText () {
		string newText = flightObject.name + ":\n";
		BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | 
			BindingFlags.Instance | BindingFlags.Static;
		PropertyInfo[] flightInputProperties = fInputs.GetType().GetProperties(flags);
		foreach (PropertyInfo propertyInfo in flightInputProperties) {
			newText += string.Format ("{0}: {1}\n", propertyInfo.Name, propertyInfo.GetValue(fInputs, null));
		}
		text.text = newText;
	}
	
	public void autoConfig() {
		if (!flightObject) {
			flightObject = GameObject.FindGameObjectWithTag ("Player");
		}

		if (!ffComponent) {
			ffComponent = flightObject.GetComponent<FreeFlight> ();
		}

		if (fInputs == null && ffComponent != null)
			fInputs = ffComponent.modeManager.flightMode.flightInputs;

		if (text == null && ffComponent != null)
			text = GetComponentInChildren<Text> ();
	}

	public void nullCheck (string name, System.Object obj) {
		if (obj == null) {
			Debug.LogError(this.name + ": " + name + " is null, please set it to something");
			this.enabled = false;
		}
	}
	

}
