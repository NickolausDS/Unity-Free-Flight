using UnityEngine;
using System.Collections;
using UnityFreeFlight;

namespace UnityFreeFlight {

	public class FlightPhysicsStats : StatsObject {

		public GameObject flightObject;
		private FreeFlight ffComponent;
		private FlightPhysics fPhysics;
		
		public override void OnEnable () {
			base.OnEnable ();
			nullCheck ("flightObject", flightObject, "Please set it to an object with a Free Flight Component");
		}
		
		public void Update () {
			updateText (fPhysics, flightObject.name);
		}
		
		public override void autoConfig() {
			base.autoConfig ();
			
			if (!flightObject) {
				flightObject = GameObject.FindGameObjectWithTag ("Player");
			}
			
			if (!ffComponent) {
				ffComponent = flightObject.GetComponent<FreeFlight> ();
			}
			
			if (fPhysics == null && ffComponent != null)
				fPhysics = ffComponent.modeManager.flightMode.flightPhysics;	
		}
	}

}
