using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace FreeFlightDemo {

	[ExecuteInEditMode]	[AddComponentMenu ("Scripts/FreeFlightDemo/MenuResizer")]
	public class MenuResizer : MonoBehaviour {

		public GameObject menu;
		public bool autoConfigMenuItems = true;
		[Tooltip ("Resize the menu based on the number of active items within it")]
		public bool shouldResizeMenu = true;
		[Tooltip ("Resize the child objects to fit within the menu")]
		public bool shouldResizeMenuContense = false;
		private RectTransform menuRectTransform;
		public List<GameObject> items = new List<GameObject>();
		public int paddingPerItem = 20;
		private int totalActiveHeight;

		void OnRenderObject() {

			if (menu == null)
				menu = gameObject;

			if (autoConfigMenuItems && menu != null) {
				resetMenuItems();
			}

			if (shouldResizeMenu && items.Count > 0) {
				resizeMenu();
			} 

			if (shouldResizeMenuContense && items.Count > 0) {
				resizeMenuContense();
			}
		}

		private void resetMenuItems() {
			menuRectTransform = menu.GetComponent<RectTransform> ();
			items.Clear ();
			totalActiveHeight = 0;
			foreach (Transform child in menu.transform) {
					items.Add (child.gameObject);
					if (child.gameObject.activeInHierarchy)
						totalActiveHeight += (int) items [0].GetComponent<RectTransform> ().sizeDelta.y;
			}


			totalActiveHeight += paddingPerItem * (getActiveItems () + 1);

		}

		private void resizeMenu() {
			menuRectTransform.sizeDelta = new Vector2 (menuRectTransform.sizeDelta.x, totalActiveHeight);	
		}

		private void resizeMenuContense() {
			List<RectTransform> rects = getActiveRectTransforms ();

			Vector2 position;
			position.x = menuRectTransform.position.x;
			position.y = menuRectTransform.position.y + menuRectTransform.sizeDelta.y/2;
			for (int i = 0; i < rects.Count; i++) {
				if (i == 0)
					//The first item gets one chunk of extra padding, and we need to offset by half the 
					//first item because objects are measured at their middle
					position = new Vector2(position.x, position.y - (paddingPerItem + rects[i].sizeDelta.y/2));
				else
					position = new Vector2(position.x, position.y - (paddingPerItem + rects[i].sizeDelta.y));
				rects[i].GetComponent<RectTransform>().position = position;
			}

		}

		private List<RectTransform> getActiveRectTransforms() {
			List<RectTransform> rt = new List<RectTransform> ();
			foreach (GameObject item in items) {
				if (item.activeInHierarchy)
					rt.Add (item.GetComponent<RectTransform> ());
			}
			return rt;
		}

		private int getActiveItems() {
			int activeItems = 0;
			foreach (GameObject item in items) {
				if (item.activeInHierarchy)
					activeItems++;
			}
			return activeItems;
		}
		
	}
}
