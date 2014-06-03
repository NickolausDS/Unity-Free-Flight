using UnityEngine;
using System.Collections;

public class InfoMenu : MonoBehaviour {

	void OnGUI() {
		
		string message = "'Esc' to pause.\n" +
			"'W A S D' to move.\n" +
			"Double Jump to fly.";
		
		GUI.BeginGroup (new Rect (Screen.width - 130, 10, 200, 90));
		GUI.Box (new Rect (0, 0, 120, 60), message);
		GUI.EndGroup ();
	}
}
