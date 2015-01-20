using UnityEngine;
using System.Collections;

/// <summary>
/// Display text to the user, in a meaningful way! This class
/// acts as a base class for configuring how the text appears.
/// The condition is undefined, and the action by default stops
/// the text from appearing.
/// 
/// The X,Y,Width,Height are modified with textPosition. Additionally,
/// you can position the text box relative to the screen:
/// RelativeToScreenWidth -- textposition.X + screen Width
/// RelativeToScreenHeight = textposition.Y + screen Height
/// </summary>
public abstract class BaseTextBlurb : BaseTrigger {

	public string textLine1;
	public string textLine2;
	public string textLine3;
	public Vector4 textPosition = new Vector4(-300, 10, 200, 60);
	public bool relativeToScreenWidth = true;
	public bool relativeToScreenHeight = false;
	
	protected bool showGUI = false;
	
	protected override void initialAction () {
		showGUI = true;
	}

	protected override void action() {
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
