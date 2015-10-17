using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityFreeFlight;

namespace UnityFreeFlight {

	/// <summary>
	/// Apply flight mechanics on a game object when enabled by the Mode Manager
	/// </summary>
	[Serializable]
	public class FlightMode : BaseMode {
	
		public FlightMechanics flightModeMechanics;
		public FlightPhysics flightModePhysics;

		public Rigidbody rigidbody;

		public override void init (GameObject go) {
			base.init (go);

			rigidbody = gameObject.GetComponent<Rigidbody> ();
			flightModePhysics.init (rigidbody);

			if (flightModeMechanics == null)
				flightModeMechanics = new FlightMechanics ();
			if (flightModePhysics == null)
				flightModePhysics = new FlightPhysics ();

			name = "Flight Mode";
			usePhysics = true;

			base.setupMechanics(flightModeMechanics, flightModePhysics);

		}


		public override void startMode () {
			base.startMode ();
			//Make sure these properties are correctly set, otherwise
			//flight with rigidbodies is impossible.

			//Drag is based on a linear model, which can't be used alongside flight
			rigidbody.drag = 0.0f;
			//Don't use rigidbody based auto rotation, it fights with physics
			rigidbody.freezeRotation = true;
			rigidbody.isKinematic = false;

			rigidbody.mass = flightModePhysics.mass;
			//Open the wings
			flightModePhysics.setWingExposure (1f, 1f);
		}

		protected override void applyPhysics () {
			flightModePhysics.applyPhysics();
		}

	}
}
