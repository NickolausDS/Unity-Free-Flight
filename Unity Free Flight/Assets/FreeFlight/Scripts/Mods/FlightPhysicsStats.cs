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
			nullCheck ("flightObject", flightObject, "Please set it to an object with a Free Flight Component");
		}
		
		public void Update () {
			defaultUpdate (fPhysics, "Flight Physics: " + flightObject.name);

			if (visualDebugging) {
				Debug.DrawRay(flightObject.transform.position, ffComponent.modeManager.flightMode.flightPhysics.liftForceVector, liftColor);
				Debug.DrawRay(flightObject.transform.position, ffComponent.modeManager.flightMode.flightPhysics.dragForceVector, dragColor);
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
			
			if (!ffComponent) {
				ffComponent = flightObject.GetComponent<FreeFlight> ();
			}

			if (!frigidbody)
				frigidbody = flightObject.GetComponent<Rigidbody> ();
			
			if (fPhysics == null && ffComponent != null)
				fPhysics = ffComponent.modeManager.flightMode.flightPhysics;	
		}
	}

}
