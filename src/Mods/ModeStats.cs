using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityFreeFlight;

namespace UnityFreeFlight {

	public class ModeStats : StatsObject {

		public GameObject flightObject;
		private FreeFlight ffComponent;
		private BaseMode fMode;
		
		public override void OnEnable () {
			base.OnEnable ();
			nullCheck ("flightObject", flightObject, "Please set it to an object with a Free Flight Component");
		}
		
		public void Update () {
			fMode = ffComponent.modeManager.currentMode;

			string stats = "";
			foreach (Mechanic mech in fMode.mechanics) {
				if (fMode.currentMechanics.Contains (mech))
				    stats += ">" + mech.GetType().Name + "\n";
				else 
					stats += mech.GetType().Name + "\n";
			}
			stats += "\n\n";
			for (LinkedListNode<Mechanic> node = fMode.currentMechanics.First; node != null; node = node.Next) {
				stats += node.Value.GetType().Name + "-->";
			}

			updateText (stats);

		}
		
		public override void autoConfig() {
			base.autoConfig ();
			
			if (!flightObject) {
				flightObject = GameObject.FindGameObjectWithTag ("Player");
			}
			
			nullCheck ("flightObject", flightObject, "Please set it to an object with a Free Flight Component");
			
			ffComponent = flightObject.GetComponent<FreeFlight> ();
			
			nullCheck ("", flightObject, "This object can only display stats info for objects with a Free Flight Component.");

		}
	}
}