using UnityEngine;
using System.Collections;

public class InfoMenu : MonoBehaviour {

	void OnGUI() {
		
		string message = "'Esc' to pause.";
		
		GUI.BeginGroup (new Rect (Screen.width - 130, 10, 200, 40));
		GUI.Box (new Rect (0, 0, 120, 25), message);
		GUI.EndGroup ();
	}
}
