using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	private bool isPaused = false;
	public GameObject playerObject = null;
	private BaseFlightController bc;

	private bool mainMenu = true;
	private bool optionsMenu = false;


	// Use this for initialization
	void Start () {
		if (playerObject != null) {
			SimpleFlight sf = playerObject.GetComponentInChildren<SimpleFlight>();
			bc = sf.flightController;
		} else {
			string msg = "The player object is not set for the in-game menu. " +
				"Please set the 'player object' in the 'pause menu' to whatever object is controlled by the player.";
			throw new UnityException (msg);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
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
		if (isPaused && mainMenu) {
			GUI.BeginGroup (new Rect (Screen.width / 2 - 125, Screen.height / 2 - 125, 250, 300));
			GUI.Box (new Rect (0, 0, 250, 500), "Menu");
			if (GUI.Button (new Rect (25, 20, 200, 50), "Resume")) {
				unpause ();
			}
			if (GUI.Button (new Rect (25, 90, 200, 50), "Options")) {
				if (playerObject) {
					mainMenu = false;
					optionsMenu = true;
				}
			}
			if (GUI.Button (new Rect (25, 160, 200, 50), "Restart")) {
				Application.LoadLevel(0);
			}
			if (GUI.Button (new Rect (25, 230, 200, 50), "Exit")) {
				Application.Quit();
			}
			GUI.EndGroup();
		} else if (isPaused && optionsMenu && playerObject) {
			GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 125, 500, 300));
			GUI.Box (new Rect (0, 0, 500, 300), "Menu");
			if (GUI.Button (new Rect (150, 240, 200, 50), "Done")) {
				mainMenu = true;
				optionsMenu = false;
			}
			bc.Inverted = GUI.Toggle (new Rect (25, 20, 200, 50), bc.Inverted, "Inverted Controls");
			GUI.EndGroup();

		}
	}
}
