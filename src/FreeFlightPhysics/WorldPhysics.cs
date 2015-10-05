

namespace UnityFreeFlight {

	/// <summary>
	/// World Physics defines all the external forces for the world, 
	/// and handles interactions of forces against flying objects.
	/// 
	/// This class works in tandem with UnityEngine.Physics, taking
	/// from it variables like gravity, and its own variables which
	/// don't exist in Unity such as air pressure
	/// </summary>
	public class WorldPhysics  {

		public static float pressure = 1.225f;

	}


}