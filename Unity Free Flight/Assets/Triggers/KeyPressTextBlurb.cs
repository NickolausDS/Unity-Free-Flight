using UnityEngine;
using System.Collections;

public class KeyPressTextBlurb : BaseTrigger {
	
	public string textLine1;
	public string textLine2;
	public string textLine3;
	public string button;
	public Vector4 textPosition = new Vector4(-300, 10, 200, 60);
	public bool relativeToScreenWidth = true;
	public bool relativeToScreenHeight = false;

	private bool showGUI = true;

	protected override void initialAction () {}

	protected override bool condition ()
	{
		if (Input.GetButton (button) )
			return true;
		return false;
	}

	protected override void action () {
		showGUI = false;
	}

	void OnGUI() {

		if (showGUI) {

			Vector4 actPos = textPosition;

			if (relativeToScreenWidth)
				actPos.x += Screen.width;
			if (relativeToScreenHeight)
				actPos.y += Screen.height;

			GUI.Box (new Rect (actPos.x, actPos.y, actPos.z, actPos.w), textLine1 + '\n' + textLine2 + '\n' + textLine3);
		}
	}
	
}
