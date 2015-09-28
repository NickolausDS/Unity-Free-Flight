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
	
		public FlightInputs flightInputs;
		public FlightMechanics flightModeMechanics;
		public FlightPhysics flightModePhysics;

		public Rigidbody rigidbody;

		public void setupMechanics() {

			foreach (Mechanic mech in mechanics) {
				mech.init (gameObject, flightModePhysics, flightInputs);
			}
			
			if (defaultMechanic != null) {
				defaultMechanic.init (gameObject, flightModePhysics, flightInputs);
			} else {
				Debug.LogError ("Default Flight Mechanic not setup!");
			}

			if (finishMechanic != null) {
				finishMechanic.init (gameObject, flightModePhysics, flightInputs);
			}

		}

		public override void init (GameObject go) {
			base.init (go);
			rigidbody = gameObject.GetComponent<Rigidbody> ();


			if (flightInputs == null)
				flightInputs = new FlightInputs ();
			if (flightModeMechanics == null)
				flightModeMechanics = new FlightMechanics ();
			if (flightModePhysics == null)
				flightModePhysics = new FlightPhysics ();

			flightModePhysics.init (rigidbody);

			inputs = flightInputs;

			name = "Flight Mode";
			usePhysics = true;

			flightModeMechanics.load<Mechanic> (defaultMechanicTypeName, ref defaultMechanic);
			flightModeMechanics.load<Mechanic> (mechanicTypeNames, ref mechanics);
			flightModeMechanics.load<Mechanic> (finishMechanicTypeName, ref finishMechanic);

			setupMechanics ();
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
		}

		protected override void applyPhysics () {
			flightModePhysics.applyPhysics();
		}

	}
}
