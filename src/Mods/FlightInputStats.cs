using UnityEngine;
using UnityFreeFlight;

namespace UnityFreeFlight {

	public class FlightInputStats : StatsObject {

		public GameObject flightObject;
		private FreeFlight ffComponent;
//		private FlightInputs fInputs;
//		private GroundInputs gInputs;

		public override void OnEnable () {
			base.OnEnable ();
		}
		
		public void Update () {
//			if (ffComponent.modeManager.activeMode == MovementModes.Flight)
//				defaultUpdate (fInputs, flightObject.name);
//			else if (ffComponent.modeManager.activeMode == MovementModes.Ground)
//				defaultUpdate (gInputs, flightObject.name);
		}

		public override void autoConfig() {
			base.autoConfig ();

			if (!flightObject) {
				flightObject = GameObject.FindGameObjectWithTag ("Player");
			}

			nullCheck ("flightObject", flightObject, "Please set it to an object with a Free Flight Component");

			ffComponent = flightObject.GetComponent<FreeFlight> ();

			nullCheck ("", ffComponent, "This object can only display stats info for objects with a Free Flight Component.");

//			fInputs = ffComponent.modeManager.flightMode.flightInputs;
//			gInputs = ffComponent.modeManager.groundMode.groundInputs;

		}

	}

}
