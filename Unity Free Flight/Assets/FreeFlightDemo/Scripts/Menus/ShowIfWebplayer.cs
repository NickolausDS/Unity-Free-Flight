using UnityEngine;
using System.Collections;

public class ShowIfWebplayer : MonoBehaviour {

	[Tooltip ("Force show object even if this isn't running inside the webplayer")]
	public bool showForTesting = false;
	// Use this for initialization
	void Start () {
		if (!Application.isWebPlayer && !showForTesting)
			gameObject.SetActive (false);
	}

}
