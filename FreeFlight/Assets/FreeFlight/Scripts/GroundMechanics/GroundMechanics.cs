using System;
using UnityFreeFlight;

namespace UnityFreeFlight {

	/// <summary>
	/// Container class for the ground mechanics.
	/// 
	/// NOTE: Due to the polymorphic serialization problem, any mechanics used by free flight
	/// have to be defined here. Variable names MUST be lower case versions of their class names.
	/// </summary>
	[Serializable]
	public class GroundMechanics : PolymorphicSerializer {
		public Idle idle = new Idle();
		public Walking walking = new Walking();
		public Launching launching = new Launching();
	}
	
}
