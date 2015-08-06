using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace FreeFlightDemo {

	[AddComponentMenu ("Scripts/FreeFlightDemo/PauseMenu")]
	public class PauseMenu : MonoBehaviour {

		[Tooltip ("Set to true of false to pause game.")]
		public bool isPaused = false;

		[Tooltip ("The Canvas that gets set to active on a pause event")]
		public GameObject mainMenu;

		[Tooltip ("GameObject with Event System Component. Used for setting selected object.")]
		public GameObject eventSystem;
		private EventSystem eventSystemComponent { 
			get { 
				if (__eventSystemComponent == null)
					__eventSystemComponent = eventSystem.GetComponent<EventSystem>();
				return __eventSystemComponent;
			}
		}
		private EventSystem __eventSystemComponent;
		private Selectable selectable;

		private GameObject activeMenu;
		public string[] availableLevels = {"MainMenu", "Grounded"};

		// Use this for initialization
		void Start () {
			if (!mainMenu)
				throw new UnityException ("Please set the main menu Canvas for: " + gameObject.name);
			activeMenu = mainMenu;
			if (isPaused)
				pause ();

			if (!eventSystem) {
				eventSystem = GameObject.Find ("EventSystem");
				if (!eventSystem)
					throw new UnityException (gameObject.name + ": Please set the 'Event System' object.");
			}

			selectable = gameObject.GetComponent<Selectable> ();
			if (!selectable)
				Debug.LogWarning ("No 'Selectable' component on " + gameObject.name + ". Selecting things on" +
				                  " this menu may not work for all devices.");
		}

		public void pause() {
			Time.timeScale = 0.0f;
			isPaused = true;
			//Disallow the pause menu from stealing interaction from menu items
			selectable.interactable = false;
			selectMenu (mainMenu);
		}

		public void unpause() {
			selectable.interactable = true;
			eventSystemComponent.UpdateModules ();
			eventSystemComponent.SetSelectedGameObject (gameObject);

			Time.timeScale = 1.0f;
			isPaused = false;
			mainMenu.SetActive (false);
		}

		public void togglePause() {
			if (!isPaused) {
				if (activeMenu == mainMenu) {
					pause ();
				} else {
					selectMenu(mainMenu);
				}
			} else {
				unpause();
			}
		}

		public void selectMenu(GameObject newMenu) {
			//Enable game object for the menu
			if (activeMenu != null)
				activeMenu.SetActive (false);
			activeMenu = newMenu;
			activeMenu.SetActive (true);

			//Search the new menu for the first 'Selectable' object and transfer
			//selection to it. We need at least one selectable object to point us
			//back to the main menu, or our selection is lost in the woods. 
			//(small hack: by counting from the highest child to zero, we start at
			//the top of the menu. I'm not sure why this is the case. It may be wise
			//to have a better definition for the "topmost" menu item in the future
			//rather than depend on this hack.) 
			for (int i = newMenu.transform.childCount - 1; i >= 0; --i) {
				Transform child = newMenu.transform.GetChild(i);
				Selectable sel = child.gameObject.GetComponent<Selectable>();
				if (sel != null && sel.interactable == true) {
					eventSystemComponent.SetSelectedGameObject (child.gameObject);
					eventSystemComponent.UpdateModules ();
				}			
			}


			if (!eventSystemComponent.currentSelectedGameObject.transform.IsChildOf(newMenu.transform))
				Debug.LogWarning ("Could not transfer selection to menu " + newMenu + ". Make sure" +
				                  " it has child menu objects with 'Selectable' components that " +
				                  " are interactable.");

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
