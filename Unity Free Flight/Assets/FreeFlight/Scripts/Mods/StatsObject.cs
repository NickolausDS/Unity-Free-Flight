using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityFreeFlight;

namespace UnityFreeFlight {

	public class StatsObject : MonoBehaviour {
	
		public Text text;
		protected string preparedStatsInfo;
		protected BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		
		// Use this for initialization
		public virtual void OnEnable () {
			autoConfig ();
			nullCheck ("text", text, gameObject.name + " missing a 'Text' UI object to display stats information");
			
		}

		/// <summary>
		/// Show all properties in an object, in whatever order they appear within that object. 
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="headerTitle">Header title.</param>
		/// <param name="additionalInformation">information appended to the stats info.</param>
		public void defaultUpdate(System.Object obj, string headerTitle, string additionalInformation="") {
			if (obj == null)
				throw new UnityException("Please set a target object for " + gameObject.name);
			
			preparedStatsInfo = headerTitle + ":\n";
			foreach (PropertyInfo propertyInfo in getProperites(obj, flags)) {
				preparedStatsInfo += string.Format ("{0}: {1}\n", propertyInfo.Name, propertyInfo.GetValue(obj, null));
			}

			updateText (preparedStatsInfo + additionalInformation);
		}
		
		public void updateText (string newtext) {
			text.text = newtext;
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

		public Dictionary<string, T> getPropertiesWithType<T>(System.Object obj)  {
			Dictionary<string, T> objects = new Dictionary<string, T> ();
			foreach (PropertyInfo prop in getProperites(obj, flags) ) {
				if (prop.PropertyType == typeof(T))
					objects.Add (prop.Name, (T) prop.GetValue(obj, null));
			}
			return objects;
		}

		public PropertyInfo[] getProperites(System.Object obj, BindingFlags bflags) { 
			return obj.GetType().GetProperties(bflags);
		}
		
		
	}
}
