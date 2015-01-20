/// <summary>
/// Display flight details about a Free Flight object.  
/// </summary>

using UnityEngine;
using System.Collections;

public class StatsView : MonoBehaviour {

	public GameObject flightObject;
	private FreeFlight ff;
	private FreeFlightPhysics ffp;
	public GUISkin guiskin;

	public bool toggleStatsMenu = false;
	public bool togglePhysicsMenu = false;
	public bool toggleWorldPhysicsMenu = false;
	public bool toggleInputsMenu = false;
	public bool showAbbreviations = true;


	public Rect statsPos = new Rect(310, 10, 200, 110);
	public Rect physicsPos = new Rect(100,10,200,140);
	public Rect worldPhysicsPos = new Rect(100, 160, 200, 90);
	public Rect InputsPos = new Rect(580, 10, 220, 190);
	
	void OnEnable() {
		if (flightObject == null) {
			string msg = "Unable to display stat information: No flight object set for " + gameObject.name;
			msg += " Please set the \'Flight Object\' variable to an object which has the \'FreeFlight\' script attached";
			Debug.LogWarning (msg);
			this.enabled = false;
		}
		if (ff == null)
			ff = flightObject.GetComponent<FreeFlight>(); 
		if (ff == null) {
			string msg = "GameObject '"+ flightObject.name + "' doesn't have the 'FreeFlight' script attached. Unable " +
				"to display flight statistics.";
			Debug.LogWarning (msg);
			this.enabled = false;
		}
		if (ff != null)
			ffp = ff.flightPhysics;
	}

	void OnGUI() {

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
			               ffp.WingSpan, ffp.getLengthType(showAbbreviations),
			               ffp.WingChord, ffp.getLengthType(showAbbreviations),
			               ffp.WingArea, ffp.getAreaType(showAbbreviations),
			               ffp.AspectRatio,
			               ffp.Weight, ffp.getWeightType(showAbbreviations)
			               ));		
			
		}

		//I'm not sure the numbers displayed are accurate. These need to be checked over.
		if (togglePhysicsMenu) {
			GUI.Box(physicsPos, string.Format(
				"Speed: {0:0.0}{1}\n" +
				"Altitude: {2:0.0}{3}\n" +
				"Lift: {4:0.0}{5}\n" +
				"Drag: {6:0.0}{7}\n" +
				"\tInduced: {8:0.0}{9}\n" +
				"\tForm: {10:0.0}{11}\n" +
				"Angle Of Attack: {12:00}{13}\n" +
				"Lift COF: {14:0.00}", 
				ffp.Speed, 			ffp.getLongDistanceType(showAbbreviations),
				ffp.VerticalSpeed, 	ffp.getShortDistanceType (showAbbreviations),
				ffp.Lift, 				ffp.getForceType (showAbbreviations),
				ffp.Drag, 				ffp.getForceType(showAbbreviations),
				ffp.LiftInducedDrag, 	ffp.getForceType(showAbbreviations),
				ffp.FormDrag, 			ffp.getForceType(showAbbreviations),
				ffp.AngleOfAttack, 	"Deg",
				ffp.LiftCoefficient)
			        );
//			Debug.Log ("Drag: " + ffp.Drag);
			if (toggleWorldPhysicsMenu) {
				GUI.Box (worldPhysicsPos, string.Format (
					"speed Vector: {0}\n" +
					"Direction {1}\n" +
					"Gravity: {2}\n",
					ffp.Velocity,
					ffp.Rotation,
					Physics.gravity.y 
					));
			}
			
			ffp.liftEnabled = GUILayout.Toggle(ffp.liftEnabled, "Lift Force");
			ffp.dragEnabled = GUILayout.Toggle(ffp.dragEnabled, "Drag Force");
			ffp.gravityEnabled = GUILayout.Toggle(ffp.gravityEnabled, "Gravity");
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

				"Wing Input (left|right) : ({6:0.0}|{7:0.0})\n" + 
				"Wing Exposure (left|right) : ({8:0.0}|{9:0.0})\n" + 
				"Bank (Input|Angle): ({10:0}|{11:00.0})\n" +
				"Pitch (Input|Angle): ({12:0}|{13:00.0})\n",

				ff.enabledFlapping,
				ff.enabledFlaring,
				ff.enabledDiving,

				ff.InputFlap,
				ff.InputFlaring,
				ff.InputDiving,

				ff.LeftWingInput, ff.RightWingInput,
				ff.LeftWingExposure, ff.RightWingExposure,
				ff.InputBank, ff.AngleBank,
				ff.InputPitch, ff.AnglePitch

				));

		}
		toggleInputsMenu = GUILayout.Toggle (toggleInputsMenu, "Player Inputs");
		
	}

}
