using UnityEngine;
using System.Collections;


public class BeginMenu : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		Time.timeScale = 0.0f;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		string message = "Hello and welcome to Unity Free Flight!\n " +
			"This is a demo designed to show off the various\n" +
			" capabilities of the free flight mechanic. It is\n" +
			"an open source package, and you can download the\n" +
			"latest version at www.windwardproductions.org.\n" +
			"Enjoy!";

		GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 125, 500, 250));
		GUI.Box (new Rect (0, 0, 500, 500), message);
		if (GUI.Button (new Rect (210, 210, 50, 20), "Play")) {
		    Time.timeScale = 1.0f;
			this.enabled = false;
		}
		GUI.EndGroup ();
	}
}


