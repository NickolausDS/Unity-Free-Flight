using UnityEngine;
using UnityFreeFlight;

namespace UnityFreeFlight {

	public class FlightInputStats : StatsObject {

		public GameObject flightObject;
		private FreeFlight ffComponent;
		private FlightInputs fInputs;

		public override void OnEnable () {
			base.OnEnable ();
			nullCheck ("flightObject", flightObject, "Please set it to an object with a Free Flight Component");
		}
		
		public void Update () {
			defaultUpdate (fInputs, flightObject.name);
		}

		public override void autoConfig() {
			base.autoConfig ();

			if (!flightObject) {
				flightObject = GameObject.FindGameObjectWithTag ("Player");
			}

			nullCheck ("flightObject", flightObject, "Please set it to an object with a Free Flight Component");

			ffComponent = flightObject.GetComponent<FreeFlight> ();

			nullCheck ("", flightObject, "This object can only display stats info for objects with a Free Flight Component.");

			fInputs = ffComponent.modeManager.flightMode.flightInputs;

		}

	}

}
