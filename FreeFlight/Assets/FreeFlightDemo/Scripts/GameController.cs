using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class GameController : MonoBehaviour {

	[Tooltip ("Normal Main Camera. Enabled by default.")]
	public GameObject normalCamera;
	[Tooltip ("Oculus Camera. Not enabled by default.")]
	public GameObject oculusCamera;
	[Tooltip ("(Optional) Needed for setting camera render on camera switch.")]
	public GameObject menu;
	[Tooltip ("(Optional) Needed for seting camera render on camera switch.")]
	public GameObject hud;
	private bool oculusEnabled;

	public void toggleOculus(bool value) {
		oculusEnabled = value;
		normalCamera.SetActive (!oculusEnabled);
		oculusCamera.SetActive (oculusEnabled);
		VRSettings.enabled = oculusEnabled;

		if (oculusEnabled) {
			if (hud != null)
				hud.GetComponent<Canvas> ().worldCamera = oculusCamera.GetComponentInChildren<Camera> ();
			if (menu != null)
				menu.GetComponent<Canvas> ().worldCamera = oculusCamera.GetComponentInChildren<Camera> ();
		}
	}

}
