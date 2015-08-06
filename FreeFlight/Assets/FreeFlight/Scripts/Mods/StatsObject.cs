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
		//This fixes a problem where the object we're 'stating' isn't finished initializing until
		//Start time. This object needs to initialize in OnEnable time, but can't start throwing errors
		//until we're sure things have loaded properly. 
		private bool hasStarted = false;
		
		// Use this for initialization
		public virtual void OnEnable () {
			try {
			autoConfig ();
			} catch (UnityException ue) {
				if (hasStarted)
					throw ue;
			}
			nullCheck ("text", text, gameObject.name + " missing a 'Text' UI object to display stats information");
			
		}

		//Doing an additional autoconfig fixes a problem where this script loads before the 
		//object we're stat-ing. This way, we'll always be stat-ing the correct objects. 
		public virtual void Start() {
			hasStarted = true;
			autoConfig ();
		}

		/// <summary>
		/// Show all properties in an object, in whatever order they appear within that object. 
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="headerTitle">Header title.</param>
		/// <param name="additionalInformation">information appended to the stats info.</param>
		public void defaultUpdate(System.Object obj, string headerTitle, string additionalInformation="") {
			nullCheck ("Internal Default Update 'obj'", obj, "Either the 'auto config' failed for this script, or it needs to be set to " +
				"execute later in 'Editor -> Project Settings -> Script Execution Order'");
			
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
				this.enabled = false;
				throw new UnityException(this.name + ": " + name + " is null. " + helpMSG);
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
