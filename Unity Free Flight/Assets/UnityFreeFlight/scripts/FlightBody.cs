using UnityEngine;
using System.Collections;

public enum Units{ Metric, Emperial };
public enum UnitTypes { Length, Area, Weight };
public enum UnitsImperial { Feet, SqFeet, Pounds };
public enum UnitsMetric {Meters, sqMeters, Kilograms };
public enum Presets { Custom, TurkeyVulture, Albatross};



//Units unit = Units.Metric;


public class FlightBody {
	
	//NOTE NOTE NOTE!!!!
	//We will always store things in METRIC
	//We convert, if we ever give things back in something else
	//NOTE NOTE NOTE!!!
	
	//1 LB == 2.20462 po
	private static float LB2KG = 0.453592f;
	private static float FT2MET = 0.3048f;

//	public enum Units{ Metric, Emperial };
	public Units unit = Units.Metric;

	private Presets _preset;

	//FLYING BODY SPECIFICATIONS
	private float _wingChord = 0f; //in meters
	private float _wingSpan = 0f;  //in meters
	private float _wingArea = 0f; // span * chord
	private float _aspectRatio = 0f; //span / chord
	
	
	private float weight;	// in kilograms
	//End flying body statistics


//Doesn't bloody work, because the editor likes to call the setter every bloody second.
	[ExposeProperty]
	public Presets Preset {
		get {return _preset; }
		set {
			_preset = value;
			switch (_preset) {
				case Presets.TurkeyVulture:
					//Turkey Vulture stats. We should eventually load this data from disk.
					_wingSpan = 1.715f;
					_wingChord = .7f;
					weight = 1.55f;
					setFromWingDimensions();
					break;
				case Presets.Albatross:
					_wingSpan = 3.5f;
					_wingChord = 0.21875f;
					weight = 11.0f;
					setFromWingDimensions();
					// also a lift to drag (L/D) of 25
					break;

			}
		}
	}

	
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
			_aspectRatio = _wingSpan / _wingChord;
		}
	}

	public void setDimensionsFromAR() {
		//derive a new wing shape without changing the wing area
		if (_wingChord > 0 && _wingSpan > 0) {
			//first, we need the initial aspect ratio.
			float initialAR = _wingChord * _wingSpan;

			_wingSpan = _aspectRatio * _wingSpan / initialAR;
			_wingChord = initialAR * _wingChord / _aspectRatio;
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
//			float initialArea = _wingChord * _wingSpan;
			
//			_wingSpan =  _wingSpan * _wingArea / initialArea;
//			_wingChord = _wingChord * _wingArea / initialArea;
			_wingSpan = Mathf.Sqrt (_wingArea * _aspectRatio);
			_wingChord = Mathf.Sqrt (_wingArea / _aspectRatio);

			//This checks to make sure wing area had been previously calculated
			//The wing area shouldn't change
			if (_aspectRatio <= 0) {
				_aspectRatio  = _wingSpan / _wingChord;
			}
		}

	}

	public bool isValid(bool log = false) {
		if(_wingSpan * _wingChord == _wingArea && _wingSpan / _wingChord == _aspectRatio) {
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
		get { return _aspectRatio; } 
		set { _aspectRatio = value;}
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
