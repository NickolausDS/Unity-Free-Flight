using UnityEngine;
using System;

namespace UnityFreeFlight {

	/// <summary>
	/// Apply ground movements when enabled by the mode manager. 
	/// </summary>
	[Serializable]
	public class GroundMode : BaseMode {

		public GroundMechanics groundModeMechanics;

		public string[] defaultMechanics = { "Launching", "Walking" };
		public string defaultDefaultMechanic = "Idle";

		public override void init (GameObject go) {
			base.init (go);

			if (groundModeMechanics == null)
				groundModeMechanics = new GroundMechanics ();

			name = "Ground Mode";

			base.setupMechanics (groundModeMechanics);
		}
	}

}
