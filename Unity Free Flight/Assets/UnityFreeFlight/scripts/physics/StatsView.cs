using UnityEngine;
using System.Collections;

//NOTE: May 1st 2014 -- Stats view will remain mostly incomplete until Flight Physics are properly refactored.

public class StatsView : MonoBehaviour {

	public GameObject flightObject;
	private FreeFlight ff;
	private FlightPhysics fPhysics;
	private FlightObject fObj;

	public bool toggleStatsMenu = false;
	public bool togglePhysicsMenu = false;
	public bool toggleWorldPhysicsMenu = false;
//	public bool toggleGravity = true;
//	public bool toggleLift = true;
//	public bool toggleDrag = true;


	void OnGUI() {

		if (!sanityCheck ())
			this.enabled = false;

		
		toggleStatsMenu = GUILayout.Toggle(toggleStatsMenu, "Show Stats");
		togglePhysicsMenu = GUILayout.Toggle(togglePhysicsMenu, "Show Physics");
		
		
		if (toggleStatsMenu) {
			GUI.Box(new Rect(310, 10, 200, 110), 
			        string.Format ("Stats:\nWing Span: {0:###.#}{1}\n " +
			               "Wing Chord: {2:###.#}{3}\n " +
			               "Total Wing Area: {4:###.#}{5}\n" +
			               "Aspect Ratio: {6:#.#}\n " +
			               "Weight: {7:###.#}{8}\n",
			               fObj.WingSpan, fObj.getLengthType(),
			               fObj.WingChord, fObj.getLengthType(),
			               fObj.WingArea, fObj.getAreaType(),
			               fObj.AspectRatio,
			               fObj.Weight, fObj.getWeightType()
			               ));		
			
		}
		
		if (togglePhysicsMenu) {
//			GUI.Box(new Rect(100,10,200,140), string.Format(
//				"Physics:\n" +
//				"Speed: {0:###.#}{1}\n" +
//				"Altitude+-: {2:###.#}{3}\n" +
//				"Lift N/H: {4:###.#}{5}\n" +
//				"Drag N/H: {6:###.#}{7}\n" +
//				"\tInduced: {8:###.#}{2}\n" +
//				"\tForm {10:###.#}{3}\n ",
//				"Angle Of Attack: {12:##}{13}\n" +
//				"Lift COF: {14:#.##}", 
//				rigidbody.velocity.magnitude * 3600.0f / 1000.0f, "KPH",
//				liftForce + Physics.gravity.y * 3600.0f / 1000.0f, "M",
//				liftForce * 3600.0f / 1000.0f, "N",
//				dragForce * 3600.0f / 1000.0f, "N",
//				fPhysics.LiftInducedDrag, "N",
//				fPhysics.FormDrag, "N",
//				angleOfAttack, "Deg",
//				liftCoefficient)
//			        );
////			if (toggleWorldPhysicsMenu) {
//				GUI.Box (new Rect(100, 160, 200, 90), string.Format (
//					"World Physics:\n" +
//					"speed Vector: {0}\n" +
//					"Direction {1}\n" +
//					"Gravity: {2}\n" +
//					"RigidBody Drag: {3} \n",
//					rigidbody.velocity,
//					rigidbody.rotation.eulerAngles,
//					Physics.gravity.y, 
//					rigidbody.drag
//					));
//			}
			
//			toggleLift = GUILayout.Toggle(toggleLift, "Lift Force");
//			toggleDrag = GUILayout.Toggle(toggleDrag, "Drag Force");
//			toggleGravity = GUILayout.Toggle(toggleGravity, "Gravity");
			toggleWorldPhysicsMenu = GUILayout.Toggle(toggleWorldPhysicsMenu, "World Physics");
		}
		
	}

	bool sanityCheck() {


		if (!flightObject) {
			Debug.LogWarning ("Unable to display stat information: No flight object set for " + gameObject.name);
			return false;
		}
		if (!ff) {
			ff = flightObject.GetComponent<FreeFlight>(); 
			if (!ff) {
				Debug.LogWarning (gameObject.name + " doesn't seem to have any flight scripts attached. Unable to display flight stats");
				return false;
			}
			fPhysics = ff.fPhysics;

		}
		return true;

	}

}
