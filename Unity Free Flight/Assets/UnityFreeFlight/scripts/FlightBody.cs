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
	public enum UnitTypes { Length, Area, Weight };
	
	public enum Presets { Custom, TurkeyVulture};
	public Presets Preset = Presets.TurkeyVulture;

	//FLYING BODY SPECIFICATIONS
	private float _wingChord = 0f; //in meters
	private float _wingSpan = 0f;  //in meters
	private float _wingArea = 0f; // span * chord
	private float aspectRatio = 0f; //span / chord
	
	
	private float weight;	// in kilograms
	//End flying body statistics
	
	void Start() {
		unit = Units.Metric;
		WingSpan = 1.715f;
		WingChord = 0.7f;
		weight = 1.55f;
		setFromWingDimensions();

	}
	
	void Update() {
		
	}

//Doesn't bloody work, because the editor likes to call the setter every bloody second.
//	[ExposeProperty]
//	public Presets Preset {
//		get {return _preset; }
//		set {
//			_preset = value;
//			if (value == Presets.TurkeyVulture) {
//				//Turkey Vulture stats. We should eventually load this data from disk.
//				WingSpan = 1.715f;
//				WingChord = .7f;
//				weight = 1.55f;
//				setFromWingDimensions();
//			}
//		}
//	}

	
	[ExposeProperty]
	public float WingChord {
		get { 
			if (unit == Units.Metric) {
				return _wingChord;
			} else {
				return _wingChord / FT2MET;
			}
		}
		set { 
			//Set in proper units
			if (unit == Units.Metric) {
				_wingChord = value;
			} else {
				_wingChord = value * FT2MET;
			}
		}
	}
	
	[ExposeProperty]
	public float WingSpan {
		get {
			if (unit == Units.Metric) {
				return _wingSpan;
			} else {
				return _wingSpan / FT2MET;
			}
		}
		set {
			//Set in proper units
			if (unit == Units.Metric) {
				_wingSpan = value;
			} else {
				_wingSpan = value * FT2MET;
			}
		}
	}

	public void setFromWingDimensions() {
		if (_wingChord > 0 && _wingSpan > 0) {
			_wingArea = _wingChord * _wingSpan;
			aspectRatio = _wingSpan / _wingChord;
		}
	}

	public void setDimensionsFromAR() {
		//derive a new wing shape without changing the wing area
		if (_wingChord > 0 && _wingSpan > 0) {
			//first, we need the initial aspect ratio.
			float initialAR = _wingChord * _wingSpan;

			_wingSpan = aspectRatio * _wingSpan / initialAR;
			_wingChord = initialAR * _wingChord / aspectRatio;
			//This checks to make sure wing area had been previously calculated
			//The wing area shouldn't change
			if (_wingArea <= 0) {
					_wingArea = _wingSpan * _wingChord;
			}
		}
	}

	public void setDimensionsFromArea() {
		//derive a new wing shape without changing the wing area
		if (_wingChord > 0 && _wingSpan > 0) {
			//first, we need the initial aspect ratio.
			float initialAR = _wingChord * _wingSpan;
			
			_wingSpan = aspectRatio * _wingSpan / initialAR;
			_wingChord = initialAR * _wingChord / aspectRatio;
			//This checks to make sure wing area had been previously calculated
			//The wing area shouldn't change
			if (_wingArea <= 0) {
				_wingArea = _wingSpan * _wingChord;
			}
		}

	}

	public bool isValid(bool log = false) {
		if(_wingSpan * _wingChord == _wingArea && _wingSpan / _wingChord == aspectRatio) {
			return true;
		} else {
			if (log == true) {
				Debug.LogWarning(string.Format("*FlightBody* has invalid wing dimensions. You can fix these via the Flight Body Editor in the inspector"));
			}	
			return false;
		}
	}
	
	[ExposeProperty]
	public float WingArea {
		get{
			if (unit == Units.Metric) {
				return _wingArea;
			} else {
				return _wingArea / FT2MET;
			}
		} 
		set{
			if (unit == Units.Metric) {
				_wingArea = value;
			} else {
				_wingArea = value * FT2MET;
			}
		}

	}
	
	[ExposeProperty]
	public float AspectRatio {
		//Dimensionless number! Yay, no converting! (wingspan / wingchord)
		get { return aspectRatio; } 
		set { aspectRatio = value;}
	}

	[ExposeProperty]
	public float Weight { 
		get{ 
			if (unit == Units.Metric) {
				return weight; 
			} else {
				return weight / LB2KG;
			}
		} 
		set{ 
			if (unit == Units.Metric) {
				weight = value;
			} else {
				weight = value * LB2KG;
			}
		}
	}

}
