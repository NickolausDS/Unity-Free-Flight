using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public bool isPaused = false;
	public GameObject playerObject = null;
	private BaseFlightController bc;

	//These automatically handle disabling mouselook on pause, if there is a mouselook script
	private MonoBehaviour mouseLookScript;
	private bool mouseLookScriptLastState;
	
	private enum Menus { None, Main, Levels, Options }
	private Menus currentMenu;
	private string[] availableLevels = {"Grounded"};


	// Use this for initialization
	void Start () {
		if (playerObject != null) {
			FreeFlight ff = playerObject.GetComponentInChildren<FreeFlight>();
			bc = ff.flightController;
			mouseLookScript = (MonoBehaviour) playerObject.GetComponent("MouseLook");
		} else {
			string msg = "The player object is not set for the in-game menu. " +
				"Please set the 'player object' in the 'pause menu' to whatever object is controlled by the player.";
			throw new UnityException (msg);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isPaused)
				unpause ();
			else
				pause ();
		}
	
	}

	void pause() {
		Time.timeScale = 0.0f;
		isPaused = true;
		currentMenu = Menus.Main;
		if (mouseLookScript) {
			mouseLookScriptLastState = mouseLookScript.enabled;
			mouseLookScript.enabled = false;
		}
	}

	void unpause() {
		Time.timeScale = 1.0f;
		isPaused = false;
		if (mouseLookScript)
			mouseLookScript.enabled = mouseLookScriptLastState;
	}

	void OnGUI() {
		if (!isPaused)
			return;

		if (currentMenu == Menus.Main) {
			GUI.BeginGroup (new Rect (Screen.width / 2 - 125, Screen.height / 2 - 125, 250, 370));
			GUI.Box (new Rect (0, 0, 250, 570), "Menu");
			if (GUI.Button (new Rect (25, 20, 200, 50), "Resume")) {
				unpause ();
			}
			if (GUI.Button (new Rect (25, 90, 200, 50), "Levels")) {
				currentMenu = Menus.Levels;
			}
			if (GUI.Button (new Rect (25, 160, 200, 50), "Options")) {
				if (playerObject) {
					currentMenu = Menus.Options;
				}
			}
			if (GUI.Button (new Rect (25, 230, 200, 50), "Restart")) {
				unpause ();
				Application.LoadLevel(Application.loadedLevel);
			}
			if (GUI.Button (new Rect (25, 290, 200, 50), "Exit")) {
				Application.Quit();
			}
			GUI.EndGroup();

		} else if (currentMenu == Menus.Levels) {
			GUI.BeginGroup (new Rect (Screen.width / 2 - 125, Screen.height / 2 - 125, 500, 370));
			GUI.Box (new Rect (0, 0, 250, 570), "Levels");
			for (int i = 0; i < availableLevels.Length; i++) {
				if (GUI.Button (new Rect (25, i*70+20 , 200, 50), availableLevels[i] )) {
					Application.LoadLevel(i);
					unpause();
				}
			}

			if (GUI.Button (new Rect (25, 290, 200, 50), "Back")) {
				currentMenu = Menus.Main;
			}
			GUI.EndGroup();


		} else if (isPaused && currentMenu == Menus.Options && playerObject) {
			GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 125, 500, 300));
			GUI.Box (new Rect (0, 0, 500, 300), "Menu");
			if (GUI.Button (new Rect (150, 240, 200, 50), "Done")) {
				currentMenu = Menus.Main;
			}
			bc.Inverted = GUI.Toggle (new Rect (25, 20, 200, 20), bc.Inverted, "Inverted Controls");
			StatsView sv = gameObject.GetComponent<StatsView>();
			if (sv) {
				sv.enabled = GUI.Toggle (new Rect (25, 40, 200, 20), sv.enabled, "Show Physics Statistics");
				sv.showAbbreviations = GUI.Toggle(new Rect (25, 60, 200, 20), sv.showAbbreviations, "Show Unit Abbreviations");
				FreeFlight ff = sv.flightObject.GetComponent<FreeFlight>();
				if (ff) {
					//Admittedly, this is a bit hacky. We convert the Unit enum to an integer, 
					//asign the selection to a string array that *matches* the enums by index, 
					//then convert the integer back to a Unit enum. Hacky, but works. Maybe it
					//would be better to add string-settable unit types to the unit converter.
					int unitSelection = (int) ff.PhysicsObject.Unit;
					string[] choices = new string[] {"Metric", "Imperial"};
					unitSelection = GUI.SelectionGrid(new Rect(25, 80, 150, 30), unitSelection, choices, 2);
					ff.PhysicsObject.Unit = (UnitConverter.Units) unitSelection;

				}
			}
			GUI.EndGroup();

		}
	}
}
