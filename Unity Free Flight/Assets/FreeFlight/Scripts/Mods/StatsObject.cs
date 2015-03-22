using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using UnityFreeFlight;

namespace UnityFreeFlight {

public class StatsObject : MonoBehaviour {
	
		public Text text;
		protected string preparedStatsInfo;
		
		// Use this for initialization
		public virtual void Start () {
			autoConfig ();
			nullCheck ("text", text, gameObject.name + " missing a 'Text' UI object to display stats information");
			
		}
		
		public void updateText (System.Object obj, string headerTitle) {
			preparedStatsInfo = headerTitle + ":\n";
			BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | 
				BindingFlags.Instance | BindingFlags.Static;
			PropertyInfo[] flightInputProperties = obj.GetType().GetProperties(flags);
			foreach (PropertyInfo propertyInfo in flightInputProperties) {
				preparedStatsInfo += string.Format ("{0}: {1}\n", propertyInfo.Name, propertyInfo.GetValue(obj, null));
			}
			text.text = preparedStatsInfo;
		}
		
		public virtual void autoConfig() {
			if (text == null)
				text = GetComponentInChildren<Text> ();
		}
		
		public void nullCheck (string name, System.Object obj, string helpMSG) {
			if (obj == null || obj.ToString().Equals ("null")) {
				Debug.LogError(this.name + ": " + name + " is null. " + helpMSG);
				this.enabled = false;
			}

		}
		
		
	}
}
