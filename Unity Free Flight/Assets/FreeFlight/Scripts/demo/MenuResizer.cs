using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MenuResizer : MonoBehaviour {

	public GameObject menu;
	public bool autoSetMenuItems = true;
	private RectTransform menuRectTransform;
	public List<GameObject> items;
	public int paddingPerItem = 20;
	public int perItemSize = 70;

	void OnRenderObject() {

		if (autoSetMenuItems && menu != null) {
			resetMenuItems();
		}

		if (items.Count > 0) {
			resizeMenu();
		}    
	}

	private void resetMenuItems() {
		menuRectTransform = menu.GetComponent<RectTransform> ();
		items.Clear ();
		foreach (Transform child in menu.transform) {
				items.Add (child.gameObject);
		}
	}

	private void resizeMenu() {
		int activeItems = 0;
		foreach (GameObject item in items) {
			if (item.activeInHierarchy)
				activeItems++;
		}
		int totalHeight = perItemSize * activeItems + (activeItems * 2 - 1) * paddingPerItem;
		menuRectTransform.sizeDelta = new Vector2 (menuRectTransform.sizeDelta.x, totalHeight);	
	}
	
}
