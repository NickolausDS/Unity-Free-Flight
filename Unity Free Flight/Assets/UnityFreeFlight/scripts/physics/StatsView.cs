using UnityEngine;
using System.Collections;

//NOTE: May 1st 2014 -- Stats view will remain mostly incomplete until Flight Physics are properly refactored.

public class StatsView : MonoBehaviour {

	public GameObject flightObject;
	private FreeFlight ff;
	private FlightPhysics fObj;
	private BaseFlightController controller;
	public GUISkin guiskin;

	public bool toggleStatsMenu = false;
	public bool togglePhysicsMenu = false;
	public bool toggleWorldPhysicsMenu = false;
	public bool toggleInputsMenu = false;
	public bool showAbbreviations = true;

	public Rect statsPos = new Rect(310, 10, 200, 110);
	public Rect physicsPos = new Rect(100,10,200,140);
	public Rect worldPhysicsPos = new Rect(100, 160, 200, 90);
	public Rect InputsPos = new Rect(600, 10, 200, 175);

	void OnGUI() {

		if (!sanityCheck ()) {
			this.enabled = false;
			return;
		}

		GUI.skin = guiskin;

		toggleStatsMenu = GUILayout.Toggle(toggleStatsMenu, "Show Stats");
		togglePhysicsMenu = GUILayout.Toggle(togglePhysicsMenu, "Show Physics");
		
		
		if (toggleStatsMenu) {
			GUI.Box(statsPos, 
			        string.Format ("Wing Span: {0:###.#}{1}\n" +
			               "Wing Chord: {2:###.#}{3}\n" +
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
			GUI.Box(physicsPos, string.Format(
				"Speed: {0:###.#}{1}\n" +
				"Altitude: {2:###.#}{3}\n" +
				"Lift: {4:###.#}{5}\n" +
				"Drag: {6:###.#}{7}\n" +
				"\tInduced: {8:###.#}{9}\n" +
				"\tForm: {10:###.#}{11}\n" +
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
				GUI.Box (worldPhysicsPos, string.Format (
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

		if (toggleInputsMenu) {
			GUI.Box (InputsPos, string.Format (
				"Flapping Enabled: {0}\n" +
				"Flaring Enabled: {1}\n" +
				"Diving Enabled: {2}\n\n" +

				"Input Flapping: {3}\n" +
				"Input Flaring: {4}\n" +
				"Input Diving: {5}\n\n" +

				"Wing Input (left|right) : ({6:#.#}|{7:#.#})\n" + 
				"Wing Exposure (left|right) : ({6:#.#}|{7:#.#})\n" + 
				"Bank (Input|Angle): ({8:#.#}|{9:#.#})\n" + 
				"Pitch (Input|Angle): ({10:#.#}|{11:#.#})\n",

				controller.enabledFlapping,
				controller.enabledFlaring,
				controller.enabledDiving,
				controller.InputFlap,
				controller.InputFlaring,
				controller.InputDiving,
				controller.LeftWingInput, controller.RightWingInput,
				controller.LeftWingExposure, controller.RightWingExposure,
				controller.InputBank, controller.AngleBank,
				controller.InputPitch, controller.AnglePitch

				));

		}
		toggleInputsMenu = GUILayout.Toggle (toggleInputsMenu, "Player Inputs");
		
	}

	bool sanityCheck() {


		if (!flightObject) {
			string msg = "Unable to display stat information: No flight object set for " + gameObject.name;
			msg += " Please set the \'Flight Object\' variable to an object which has the \'FreeFlight\' script attached";
			Debug.LogWarning (msg);
			return false;
		}
		if (!ff) {
			ff = flightObject.GetComponent<FreeFlight>(); 
			if (!ff) {
				string msg = "GameObject '"+ flightObject.name + "' doesn't have the 'FreeFlight' script attached. Unable " +
					"to display flight statistics.";
				Debug.LogWarning (msg);
				return false;
			}
			fObj = ff.PhysicsObject;
		}
		if (!controller) {
			controller = ff.GetComponent<BaseFlightController>();
			if (!controller) {
				string msg = "GameObject '"+ flightObject.name + "' doesn't have a controller attached. Unable to show inputs.";
				Debug.LogWarning (msg);
				return false;		
			}
		}

		return true;

	}

}
