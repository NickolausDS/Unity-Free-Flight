using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace UnityFreeFlight {

	/// <summary>
	/// This class holds child class data of a particular type, which can be loaded back into 
	/// a polymorphic parent class by class name. Since Unity does not support polymorphic
	/// serialization, this makes it possible to save child classes, and take advantage of the
	/// polymorphic nature of object oriented programming through the editor. However, this 
	/// class does not support dynamic variables; Only objects explicitly defined within this
	/// class can be 'loaded' by name.
	/// </summary>
	[Serializable]
	public class PolymorphicSerializer {

		private FieldInfo[] fields;

		/// <summary>
		/// Load the specified typeName into obj.
		/// </summary>
		/// <param name="typeName">Type name.</param>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <returns> True when data is loaded, false when data loaded equals obj</returns>
		public bool load<T>(string typeName, ref T obj) {
			T loadedObj = (T) getField (typeName, this);

			if (!loadedObj.Equals (obj)) {
				obj = loadedObj;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Load the specified typeNames into obj.
		/// </summary>
		/// <param name="typeName">Type name.</param>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <returns> True when data is loaded, false when data loaded equals obj</returns>
		public bool load<T>(List<string> typeNames, ref List<T> obj) {
			bool retval = false;
			List<T> loadedObjArray = new List<T> ();
			T next;
			for (int i = 0; i < typeNames.Count; i++) {
				//Get the next object in the obj list to check
				next = (obj != null && i < obj.Count) ? obj[i] : default(T);
				//load the next object
				retval = (load(typeNames[i], ref next) || retval);
				//put the next object in the loaded list, whether it was new or not
				loadedObjArray.Add (next);
			}

			//If we loaded new data, set the object given to the loaded data
			if (retval) {
				obj = loadedObjArray;
			}

			return retval;
		}
		
		private System.Object getField(string fieldName, System.Object obj) {
			foreach (var field in getFields(obj)) {
				//				Debug.Log ("Prop: " + field.Name);
				if (field.Name.ToLower().Equals (fieldName.ToLower())) {
					//					Debug.Log ("Found Mechanic: " + field.Name);
					return field.GetValue (obj);
				}
			}
			return null;
		}

		private FieldInfo[] getFields(System.Object obj) {
			if (fields == null)
				fields = obj.GetType().GetFields(BindingFlags.Instance|BindingFlags.Public);
			return fields;
		}
		
	}


}