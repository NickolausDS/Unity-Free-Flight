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

			name = "Ground Mode";

			base.setupMechanics (groundModeMechanics, groundInputs);
		}
	}

}
