using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;

namespace UnityFreeFlight {

	/// <summary>
	/// Apply ground movements when enabled by the mode manager. 
	/// </summary>
	[Serializable]
	public class GroundMode : BaseMode {

		public GroundInputs groundInputs;
		public GroundMechanics groundModeMechanics;

		public override void init (GameObject go) {
			base.init (go);
			
			if (groundInputs == null)
				groundInputs = new GroundInputs ();
			if (groundModeMechanics == null)
				groundModeMechanics = new GroundMechanics ();

			inputs = groundInputs;
			
			name = "Ground Mode";
			
			groundModeMechanics.load<Mechanic> (defaultMechanicTypeName, ref defaultMechanic);
			groundModeMechanics.load<Mechanic> (mechanicTypeNames, ref mechanics);
			groundModeMechanics.load<Mechanic> (finishMechanicTypeName, ref finishMechanic);
			
			setupMechanics ();
		}

		public void setupMechanics() {
			
			foreach (Mechanic mech in mechanics) {
				mech.init (gameObject, null, groundInputs);
			}
			
			if (defaultMechanic != null) {
				defaultMechanic.init (gameObject, null, groundInputs);
			} else {
				Debug.LogError ("Default Ground Mechanic not setup!");
			}
			
			if (finishMechanic != null) {
				finishMechanic.init (gameObject, null, groundInputs);
			}
			
		}




	}

}
