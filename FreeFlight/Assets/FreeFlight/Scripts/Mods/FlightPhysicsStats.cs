using UnityEngine;
using System.Collections;
using UnityFreeFlight;

namespace UnityFreeFlight {

	public class FlightPhysicsStats : StatsObject {

		public GameObject flightObject;
		private FreeFlight ffComponent;
		private FlightPhysics fPhysics;
		private Rigidbody frigidbody;
		public bool visualDebugging = false;
		public bool logFrameDebugging = false;
		public Color liftColor = Color.blue;
		public Color dragColor = Color.red;
		public Color velocityColor = Color.green;

		public override void OnEnable () {
			base.OnEnable ();
		}
		
		public void Update () {
			defaultUpdate (fPhysics, "Flight Physics: " + flightObject.name);

			if (visualDebugging && frigidbody != null) {
				Debug.DrawRay(flightObject.transform.position, ffComponent.modeManager.flightMode.flightModePhysics.liftForceVector, liftColor);
				Debug.DrawRay(flightObject.transform.position, ffComponent.modeManager.flightMode.flightModePhysics.dragForceVector, dragColor);
				Debug.DrawRay(flightObject.transform.position, frigidbody.velocity, velocityColor);
			}

			if (logFrameDebugging)
				Debug.Log (preparedStatsInfo);
		}

		
		public override void autoConfig() {
			base.autoConfig ();

			if (!flightObject) {
				flightObject = GameObject.FindGameObjectWithTag ("Player");
			}

			nullCheck ("flightObject", flightObject, "Please set it to an object with a Free Flight Component");
			
			ffComponent = flightObject.GetComponent<FreeFlight> ();
			frigidbody = flightObject.GetComponent<Rigidbody> ();

			nullCheck ("Free Flight Component", ffComponent, "Please add a Free Flight component in order to show physics stats");

			fPhysics = ffComponent.modeManager.flightMode.flightModePhysics;
		}
	}

}
