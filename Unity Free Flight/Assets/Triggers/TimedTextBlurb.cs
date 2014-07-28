using UnityEngine;
using System.Collections;

/// <summary>
/// Display a message which will automatically disappear.
/// </summary>
public class TimedTextBlurb : BaseTextBlurb {

	public float delayTime = 2.0f;
	private float startTime;

	protected override void initialAction () {
		startTime = Time.time;
		showGUI = false;
	}

	protected override bool condition ()
	{
		if (Time.time > startTime + delayTime) {
			showGUI = true;
			return true;
		}
		return false;
	}

	protected override void action () {}
}
