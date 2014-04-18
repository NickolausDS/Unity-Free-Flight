using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	private bool isPaused = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Debug.Log ("Paused!");
			pause ();
		}
	
	}

	void pause() {
		Time.timeScale = 0.0f;
		isPaused = true;
	}

	void unpause() {
		Time.timeScale = 1.0f;
		isPaused = false;
	}

	void OnGUI() {
		if (isPaused) {
			GUI.BeginGroup (new Rect (Screen.width / 2 - 125, Screen.height / 2 - 125, 250, 250));
			GUI.Box (new Rect (0, 0, 250, 500), "Menu");
			if (GUI.Button (new Rect (25, 20, 200, 50), "Resume")) {
				unpause ();
			}
			if (GUI.Button (new Rect (25, 90, 200, 50), "Restart")) {
				Application.LoadLevel(0);
			}
			if (GUI.Button (new Rect (25, 160, 200, 50), "Exit")) {
				Application.Quit();
			}
			GUI.EndGroup();
		}
	}
}
