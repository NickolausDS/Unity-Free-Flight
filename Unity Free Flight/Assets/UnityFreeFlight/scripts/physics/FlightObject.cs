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
		leftWingExposure = cleftWingExposure;
		rightWingExposure = crightWingExposure;
		currentWingArea = _wingSpan * _wingChord * 2 * (leftWingExposure + rightWingExposure);

	}

	public float WingSpan {
		get { 
			return getLength (_wingSpan); 
		}
		set {
			_wingSpan = setLength (value);
		}
	}

	public float WingChord {
		get { 
			return getLength (_wingChord); 
		}
		set {
			_wingChord = setLength (value);
		}
	}

	public float WingArea {
		get{
			return getArea (_wingArea );
		} 
		set{
			_wingArea = setArea (value);
		}
		
	}
	
	public float AspectRatio {
		//Dimensionless number! Yay, no converting! (wingspan / wingchord)
		get { return _aspectRatio; } 
		set { _aspectRatio = value;}
	}
	
	public float Weight { 
		get{ 
			return getWeight(_weight);
		} 
		set{ 
			_weight = setWeight (value);
		}
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
