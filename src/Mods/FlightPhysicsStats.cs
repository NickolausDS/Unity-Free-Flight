using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityFreeFlight;

namespace UnityFreeFlight {

	public class FlightPhysicsStats : StatsObject {

		public GameObject flightObject;
		private FreeFlight ffComponent;
		private FlightPhysics fPhysics;
		private Rigidbody frigidbody;

		[Header("Log Debugging")]
		[Tooltip("Log all field information per frame")]
		public bool logFrameDebugging = false;
		[Tooltip("Log force information once forces exceed a set level.")]
		public bool logHighForceValues = false;
		public float logForceLevel = 20f;


		[Header("Visual Debugging")]
		[Tooltip("Draw colored lines on flight object to represent visual acting forces")]
		public bool visualDebugging = false;
		public Color liftColor = Color.blue;
		public Color dragColor = Color.red;
		public Color velocityColor = Color.green;

		public override void OnEnable () {
			base.OnEnable ();
		}
		
		public void Update () {
			defaultUpdate (fPhysics, "Flight Physics: " + flightObject.name);

			if (visualDebugging && frigidbody != null) {
				Debug.DrawRay(flightObject.transform.position, fPhysics.liftForceVector, liftColor);
				Debug.DrawRay(flightObject.transform.position, fPhysics.dragForceVector, dragColor);
				Debug.DrawRay(flightObject.transform.position, frigidbody.velocity, velocityColor);
			}

			if (logHighForceValues) {
				List<string> linearForces = new List<string> ();
				linearForces.Add ("liftForce");
				linearForces.Add ("formDragForce");
				linearForces.Add ("liftInducedDragForce");
				linearForces.Add ("airspeed");
				foreach (string linearForce in linearForces) {
					float forcelevel = (float) typeof(FlightPhysics).GetProperty(linearForce).GetValue(fPhysics,null);
					if (forcelevel > logForceLevel)
						Debug.LogWarning(string.Format ("{0} Force '{1}' reached {2}!", 
						                                Time.realtimeSinceStartup, linearForce, forcelevel));
				}

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
