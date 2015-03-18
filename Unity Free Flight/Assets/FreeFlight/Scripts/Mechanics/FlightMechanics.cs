using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityFreeFlight;

namespace UnityFreeFlight {

	[Serializable]
	public class FlightMechanics : PolymorphicSerializer {
		/// <summary>
		/// Special mechanics need to be explicitly defined. This is (a hack) because
		/// Unity dosen't support polymorphic serialization for basic classes, 
		/// and doing so with scriptable objects would result in a stupid user
		/// experience.
		/// </summary>
		public Gliding gliding = new Gliding ();
		public Flaring flaring = new Flaring ();
		public Diving diving = new Diving ();
		public Flapping flapping = new Flapping ();

		public void load(BaseMode bm) {
			setupMechanics (bm);
		}

		public void setupMechanics(BaseMode bm) {
//			Debug.Log ("Setting up mechanics for base mode: " + bm.GetType().Name);
			bm.mechanics.Clear ();
			
			Mechanic newMech;
			foreach (string mechName in bm.mechanicTypeNames) {
				newMech = (Mechanic) getField (mechName, this);
				
				if (newMech != null)
					bm.mechanics.Add ( newMech );
				else 
					Debug.LogError("Failed to add mechanic for " + mechName);
			}
			
			bm.defaultMechanic = (Mechanic) getField (bm.defaultMechanicTypeName, this);
			if (bm.defaultMechanic == null)
				Debug.LogError (string.Format("Could not setup default mechanic for {0}: {1} does not seem to exist", 
				                              bm.GetType().Name, bm.defaultMechanicTypeName));
//			else 
//				Debug.Log ("Default Mechanic set to: " + bm.defaultMechanic.GetType ().Name);
			
			//Testing
//			foreach (Mechanic mech in bm.mechanics) {
//				Debug.Log ("TESTING TYPE: " + mech.GetType().Name);
//			}
			
		}
		
		private System.Object getField(string fieldName, System.Object obj) {
			foreach (var field in obj.GetType().GetFields(BindingFlags.Instance|BindingFlags.Public)) {
				//				Debug.Log ("Prop: " + field.Name);
				if (field.Name.ToLower().Equals (fieldName.ToLower())) {
//					Debug.Log ("Found Mechanic: " + field.Name);
					return field.GetValue (obj);
				}
			}
			return null;
		}

	}

}
