using UnityEngine;
using System.Collections;

public class FlightBody : MonoBehaviour {
	
	//NOTE NOTE NOTE!!!!
	//We will always store things in METRIC
	//We convert, if we ever give things back in something else
	//NOTE NOTE NOTE!!!
	
	//1 LB == 2.20462 po
	private static float LB2KG = 0.453592f;
	private static float FT2MET = 0.3048f;

	public enum Units{ Metric, Emperial };
	public Units unit = Units.Metric;
	
	public enum Presets { Custom, TurkeyVulture};

	public Presets preset = Presets.TurkeyVulture;
	//FLYING BODY SPECIFICATIONS
	private float wingChord = 0f; //in meters
	private float wingSpan = 0f;  //in meters
	private float wingArea = 0f; // span * chord
	private float aspectRatio = 0f; //span / chord
	
	
	private float weight;	// in kilograms
	private float liftToWeightRatio;
	//	private float liftToWeightRatio; // will be important, not using it now.
	//End flying body statistics
	
	void Start() {
		//Turkey Vulture stats. We'll modulalize this later.
		WingSpan = 1.715f;
		WingChord = .7f;
		weight = 1.55f;
	}
	
	void Update() {
		
	}
	
	[ExposeProperty]
	public float WingChord {
		get { 
			if (unit == Units.Metric) {
				return wingChord;
			} else {
				return wingChord / FT2MET;
			}
		}
		set { 
			//Set in proper units
			if (unit == Units.Metric) {
				wingChord = value;
			} else {
				wingChord = value * FT2MET;
			}
			//Fill in the rest, if it is non-zero
			if (wingChord > 0 && wingSpan > 0) {
				wingArea = wingChord * wingSpan;
				aspectRatio = wingSpan / wingChord;
			}
		}
	}
	
	[ExposeProperty]
	public float WingSpan {
		get {
			if (unit == Units.Metric) {
				return wingSpan;
			} else {
				return wingSpan / FT2MET;
			}
		}
		set {
			//Set in proper units
			if (unit == Units.Metric) {
				wingSpan = value;
			} else {
				wingSpan = value * FT2MET;
			}
			if (wingChord > 0 && wingSpan > 0) {
				wingArea = wingChord * wingSpan;
				aspectRatio = wingSpan / wingChord;
			}
		}
	}
	
	[ExposeProperty]
	public float WingArea {get{return wingArea;} set{wingArea = value;}}
	
	[ExposeProperty]
	public float AspectRatio { get { return aspectRatio; } set { aspectRatio = value; } }

	[ExposeProperty]
	public float Weight { get{ return weight; } set{ weight = value; } }

	[ExposeProperty]
	public float LiftToWeightRatio { get; set; }
}
