using UnityEngine;
using System.Collections;


namespace UnityFreeFlight {

	/// <summary>
	/// Inputs is intended to gather input directly from the player, and hold it in "states"
	/// which can be drawn from later. 
	/// </summary>
	public abstract class Inputs {

		/// <summary>
		/// Get inputs from the player
		/// </summary>
		public abstract void getInputs();

		/// <summary>
		/// Reset all input states back to their inital states. The reason is usually when we switch
		/// "input modes" to something different, we don't want these inputs to persist.
		/// </summary>
		public abstract void resetInputs ();

		
	}

}
