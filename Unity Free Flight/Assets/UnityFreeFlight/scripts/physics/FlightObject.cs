using UnityEngine;
using System.Collections;



//Units unit = UnitConverter.Units.Metric;


public class FlightObject : UnitConverter {
	
	//NOTE NOTE NOTE!!!!
	//We will always store things in METRIC
	//We convert, if we ever give things back in something else
	//NOTE NOTE NOTE!!!

	
	public Units Unit {
		get {return _unit;}
		set {_unit = value;}
	}

	public enum Presets { Custom, TurkeyVulture, Albatross};
	private Presets _preset;

	//FLYING BODY SPECIFICATIONS
	private float _wingChord; //in meters
	private float _wingSpan;  //in meters
	private float _wingArea; // span * chord
	private float _aspectRatio; //span / chord
	private float _weight;	// in kilograms
	//End flying body statistics

	//Dimensions which will be needed by physics. They must always be in metric, so
	//we can never have them be public.
	//current wing area is calculated from the base area and percentage of wings open
	//It is always calculated as its set, so this will always be the latest value.
	protected float currentWingArea;
	protected float leftWingExposure;
	protected float rightWingExposure;
		

	public FlightObject () {
		Preset = Presets.TurkeyVulture;
		setWingPosition (1.0f, 1.0f);
	}


//Doesn't bloody work, because the editor likes to call the setter every bloody second.
	public Presets Preset {
		get {return _preset; }
		set {
			_preset = value;
			switch (_preset) {
				case Presets.TurkeyVulture:
					//Turkey Vulture stats. We should eventually load this data from disk.
					_wingSpan = 1.715f;
					_wingChord = .7f;
					_weight = 1.55f;
					setFromWingDimensions();
					break;
				case Presets.Albatross:
					_wingSpan = 3.5f;
					_wingChord = 0.21875f;
					_weight = 11.0f;
					setFromWingDimensions();
					// also a lift to drag (L/D) of 25
					break;

			}
		}
	}

	public void setWingPosition(float cleftWingExposure, float crightWingExposure) {
		//Make sure area is never actually zero, as this is technically impossible and 
		//causes physics to fail.
		if (cleftWingExposure == 0.0f)
			leftWingExposure = 0.01f;
		else
			leftWingExposure = cleftWingExposure;
		if (crightWingExposure == 0.0f)
			rightWingExposure = 0.01f;
		else
			rightWingExposure = crightWingExposure;
		currentWingArea = _wingSpan * _wingChord * (leftWingExposure + rightWingExposure) / 2;

	}


	/// <summary>
	/// Check if the object's wings are completely extended
	/// </summary>
	/// <returns><c>true</c>, if wings are completely open, <c>false</c> otherwise.</returns>
	public bool wingsOpen() {
		if (currentWingArea == _wingArea)
			return true;
		return false;
	}

	public float LeftWingExposure { get { return leftWingExposure; } }
	public float RightWingExposure { get { return rightWingExposure; } }

	public float WingSpan {
		get {return convert (Units.Metric, _unit, Types.Length, _wingSpan) * (leftWingExposure + rightWingExposure) / 2;}
		set {_wingSpan = convert (_unit, Units.Metric, Types.Length, value);}
	}

	public float WingChord {
		get {return convert(Units.Metric, _unit, Types.Length, _wingChord) * (leftWingExposure + rightWingExposure) / 2;}
		set {_wingChord = convert (_unit, Units.Metric, Types.Length, value);}
	}

	public float WingArea {
		get{return convert (Units.Metric, _unit, Types.Area, _wingArea) * (leftWingExposure + rightWingExposure) / 2;} 
		set{_wingArea = convert (_unit, Units.Metric, Types.Area, value);}
	}
	
	public float AspectRatio {
		//Dimensionless number! Yay, no converting! (wingspan / wingchord)
		get { return _aspectRatio; } 
		set { _aspectRatio = value;}
	}
	
	public float Weight { 
		get{return convert (Units.Metric, _unit, Types.Weight, _weight);} 
		set{_weight = convert (_unit, Units.Metric, Types.Weight, value);}
	}

	public void setFromWingDimensions() {
		if (_wingChord > 0 && _wingSpan > 0) {
				_wingArea = _wingChord * _wingSpan;
				_aspectRatio = _wingSpan / _wingChord;
		} else {
			throw new UnityException("Wing Span and Wing Chord must be greator than zero");
		}
	}

	public void setWingDimensions() {
		if (_aspectRatio > 0 && _wingArea > 0) {
			_wingSpan = Mathf.Sqrt (_wingArea * _aspectRatio);
			_wingChord = Mathf.Sqrt (_wingArea / _aspectRatio);
		} else {
			throw new UnityException("Aspect Ratio and Wing Area must be greator than zero");
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


}
