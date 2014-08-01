using UnityEngine;
using System.Collections;

public class InfoMenu : MonoBehaviour {

	void OnGUI() {
		
		string message = "'Esc' to pause.\n" +
						"'W A S D' to move.\n";
		
		GUI.BeginGroup (new Rect (Screen.width - 140, 10, 200, 200));
		GUI.Box (new Rect (0, 0, 130, 40), message);
		GUI.EndGroup ();
	}
}
