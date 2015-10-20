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

			string inputs = buildMechanicInputStatuses(fMode);
			string chain = buildMechanicChain(fMode);

			string stats = string.Format ("{0}:\n'> ' == mechanic running \n '! ' == mechanic overruled\n\nInputs: \n{1}\n\nChain: \n{2}",
			               fMode.name, inputs, chain);

			updateText (stats);

		}

		public string buildMechanicInputStatuses(BaseMode mode) {
			string stats = "";
			foreach (Mechanic mech in mode.mechanics) {
				if (mode.currentMechanics.Contains (mech))
					stats += string.Format ("> :{0} \n", mech.GetType().Name);
				else if (mech.FFInputSatisfied()) {
					stats += string.Format ("! :{0} \n", mech.GetType().Name);
				}
				else 
					stats += string.Format("{0} \n", mech.GetType().Name);
			}
			return stats;
		}

		public string buildMechanicChain(BaseMode mode) {
			string stats = "";
			for (LinkedListNode<Mechanic> node = mode.currentMechanics.First; node != null; node = node.Next) {
				stats += node.Value.GetType().Name + "-->";
			}
			return stats;
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