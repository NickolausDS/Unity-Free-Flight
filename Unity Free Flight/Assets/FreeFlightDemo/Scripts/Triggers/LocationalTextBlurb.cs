using UnityEngine;
using System.Collections;

/// <summary>
/// Pops up text if object1 collides with object2
/// </summary>
public class LocationalTextBlurb : BaseTextBlurb {
	

	public GameObject location;
	public bool persistForever = false;
	private bool inLocation = false;

	protected override bool condition ()
	{
		if (!persistForever && inLocation) {
			return true;
		}
		return false;
	}

	public void OnCollision(Collision c) {

		if (c.gameObject == location) {
			showGUI = true;
		}

	}

}
