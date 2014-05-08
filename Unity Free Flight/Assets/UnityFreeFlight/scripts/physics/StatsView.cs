using UnityEngine;
using System.Collections;

//NOTE: May 1st 2014 -- Stats view will remain mostly incomplete until Flight Physics are properly refactored.

public class StatsView : MonoBehaviour {

	public GameObject flightObject;
	private FreeFlight ff;
	private FlightPhysics fObj;

	public bool toggleStatsMenu = false;
	public bool togglePhysicsMenu = false;
	public bool toggleWorldPhysicsMenu = false;
	public bool showAbbreviations = true;


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
			               fObj.WingSpan, fObj.getLengthType(showAbbreviations),
			               fObj.WingChord, fObj.getLengthType(showAbbreviations),
			               fObj.WingArea, fObj.getAreaType(showAbbreviations),
			               fObj.AspectRatio,
			               fObj.Weight, fObj.getWeightType(showAbbreviations)
			               ));		
			
		}

		//I'm not sure the numbers displayed are accurate. These need to be checked over.
		if (togglePhysicsMenu) {
			GUI.Box(new Rect(100,10,200,140), string.Format(
				"Physics:\n" +
				"Speed: {0:###.#}{1}\n" +
				"Altitude: {2:###.#}{3}\n" +
				"Lift: {4:###.#}{5}\n" +
				"Drag: {6:###.#}{7}\n" +
				"\tInduced: {8:###.#}{9}\n" +
				"\tForm: {10:###.#}{11}\n " +
				"Angle Of Attack: {12:##}{13}\n" +
				"Lift COF: {14:#.##}", 
				fObj.Speed, 			fObj.getLongDistanceType(showAbbreviations),
				fObj.VerticalSpeed, 	fObj.getShortDistanceType (showAbbreviations),
				fObj.Lift, 				fObj.getForceType (showAbbreviations),
				fObj.Drag, 				fObj.getForceType(showAbbreviations),
				fObj.LiftInducedDrag, 	fObj.getForceType(showAbbreviations),
				fObj.FormDrag, 			fObj.getForceType(showAbbreviations),
				fObj.AngleOfAttack, 	"Deg",
				fObj.LiftCoefficient)
			        );
			if (toggleWorldPhysicsMenu) {
				GUI.Box (new Rect(100, 160, 200, 90), string.Format (
					"World Physics:\n" +
					"speed Vector: {0}\n" +
					"Direction {1}\n" +
					"Gravity: {2}\n",
					fObj.Velocity,
					fObj.Rotation,
					Physics.gravity.y 
					));
			}
			
			fObj.liftEnabled = GUILayout.Toggle(fObj.liftEnabled, "Lift Force");
			fObj.dragEnabled = GUILayout.Toggle(fObj.dragEnabled, "Drag Force");
			fObj.gravityEnabled = GUILayout.Toggle(fObj.gravityEnabled, "Gravity");
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
			fObj = ff.PhysicsObject;
		}
		return true;

	}

}
