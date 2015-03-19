using System;
using UnityFreeFlight;

namespace UnityFreeFlight {

	[Serializable]
	public class FlightMechanics : PolymorphicSerializer {
		/// <summary>
		/// Special mechanics need to be explicitly defined. This is (a hack) because
		/// Unity dosen't support polymorphic serialization for basic classes, 
		/// and doing so with scriptable objects would result in a stupid user
		/// experience.
		/// </summary>
		public Gliding gliding = new Gliding ();
		public Flaring flaring = new Flaring ();
		public Diving diving = new Diving ();
		public Flapping flapping = new Flapping ();
	}

}
