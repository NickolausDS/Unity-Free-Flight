using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace FreeFlightDemo {

	[AddComponentMenu ("Scripts/FreeFlightDemo/PauseMenu")]
	public class PauseMenu : MonoBehaviour {

		[Tooltip ("Set to true of false to pause game.")]
		public bool isPaused = false;

		[Tooltip ("The Canvas that gets set to active on a pause event")]
		public GameObject mainMenu;

		private GameObject activeMenu;
		public string[] availableLevels = {"MainMenu", "Grounded"};

		// Use this for initialization
		void Start () {
			if (!mainMenu)
				throw new UnityException ("Please set the main menu Canvas for: " + gameObject.name);
			activeMenu = mainMenu;
			if (isPaused)
				pause ();
		}
		
		// Update is called once per frame
		void Update () {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (isPaused) {
					if (activeMenu == mainMenu)
						unpause ();
					else 
						selectMenu(mainMenu);
				}
				else
					pause ();
			}
		
		}

		public void pause() {
			Time.timeScale = 0.0f;
			isPaused = true;
			selectMenu (mainMenu);
		}

		public void unpause() {
			Time.timeScale = 1.0f;
			isPaused = false;
			mainMenu.SetActive (false);
		}

		public void selectMenu(GameObject newMenu) {
			if (activeMenu != null)
				activeMenu.SetActive (false);
			activeMenu = newMenu;
			activeMenu.SetActive (true);       
		}

		public void loadLevel(int level) {
			loadLevel (availableLevels [level]);
		}

		public void loadLevel(string level) {
			Application.LoadLevel (level);
		}

		public void restartLevel() {
			Application.LoadLevel (Application.loadedLevel);
		}

		public void quit() {
			Application.Quit ();
		}

	}
}
