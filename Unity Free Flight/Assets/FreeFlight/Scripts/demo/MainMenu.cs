using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FreeFlightDemo {

	public class MainMenu : MonoBehaviour {

		public GameObject playButton;
		public GameObject exitButton;
		public GameObject loadLevelOutputText;

		public bool disableWebPlayButtonUntilLevelLoad = true;
		public bool disableWebExitButton = true;

		public const int defaultLevel = 1;

		public void Start() {
			if (disableWebPlayButtonUntilLevelLoad && Application.isWebPlayer)
				playButton.GetComponent<Button>().interactable = false;
			if (disableWebExitButton && (Application.isEditor || Application.isWebPlayer) && exitButton)
				exitButton.SetActive (false);
		}

		public void Update() {

			if (disableWebPlayButtonUntilLevelLoad && Application.isWebPlayer && loadLevelOutputText)
				updateLoadStatus ();
		}

		public void ButtonStart(int level = defaultLevel) {
			Application.LoadLevel (level);
		}

		public void ButtonQuit() {
			Application.Quit ();
		}

		private void updateLoadStatus() {
			float progress = Application.GetStreamProgressForLevel (defaultLevel);
			
			if (loadLevelOutputText) {
				if (progress == 1f)
					loadLevelOutputText.GetComponent<Text>().text = "";
				else 
					loadLevelOutputText.GetComponent<Text>().text = string.Format ("Level loaded: {0:P0}", progress);
			}
			
			if (playButton) {
				if (progress < 1f)
					playButton.GetComponent<Button>().interactable = false;
				else
					playButton.GetComponent<Button>().interactable = true;
			}
		}
	}
}
