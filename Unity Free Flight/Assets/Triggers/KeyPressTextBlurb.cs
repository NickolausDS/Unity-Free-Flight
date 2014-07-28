using UnityEngine;
using System.Collections;

/// <summary>
/// Display text until the user presses a button.
/// </summary>
public class KeyPressTextBlurb : BaseTextBlurb {
	

	public string button;


	protected override bool condition ()
	{
		if (Input.GetButton (button) )
			return true;
		return false;
	}
	

}
