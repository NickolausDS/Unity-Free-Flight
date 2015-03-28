using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FreeFlightDemo {

	[AddComponentMenu ("Scripts/FreeFlightDemo/MainMenu")]
	public class MainMenu : MonoBehaviour {

		[Tooltip ("Attach a play button to disable it until level load for web builds")]
		public GameObject playButton;
		[Tooltip ("Attach the exit button to disable it for web builds")]
		public GameObject exitButton;
		[Tooltip ("The UI Text for percentage 'loading text' to be displayed")]
		public GameObject loadLevelOutputText;

		[Tooltip ("Disables play button on web build until level is loaded")]
		public bool disableWebPlayButtonUntilLevelLoad = true;
		[Tooltip ("Disables exit button on web build")]
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
