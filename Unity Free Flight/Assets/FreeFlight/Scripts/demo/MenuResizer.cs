using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuResizer : MonoBehaviour {

	public RectTransform resizableCanvas;
	public List<GameObject> items;
	public int paddingPerItem = 20;
	public int perItemSize = 70;

	void Start() {
		resizableCanvas = gameObject.GetComponent<RectTransform> ();
		foreach (Transform child in transform) {
			items.Add (child.gameObject);
		}

	}

	// Update is called once per frame
	void Update () {
		int activeItems = 0;
		foreach (GameObject item in items) {
			if (item.activeInHierarchy)
				activeItems++;
		}
		int totalHeight = perItemSize * activeItems + (activeItems * 2 - 1) * paddingPerItem;
		resizableCanvas.sizeDelta = new Vector2 (resizableCanvas.sizeDelta.x, totalHeight);
	}
}
