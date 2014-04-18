using UnityEngine;
using System.Collections;
using System;


public class UnitConverter {

	public enum Units{ Metric, Imperial };
	public enum Types { Length, Area, Weight };
//	public enum Imperial { Feet, SqFeet, Pounds };
//	public enum Metric {Meters, SqMeters, Kilograms };

	public static string[] imperial = {"FT", "FT^2", "LB"};
	public static string[] metric = {"M", "M^2", "KG"};

	private static float METSQ2FTSQ = 10.764f;
	private static float MET2FT = 3.28084f;
	private static float KG2LB = 2.20462f;


	public Units _unit = Units.Metric;

	public UnitConverter(Units theunit) {
		_unit = theunit;
	}

	public UnitConverter() {_unit = Units.Metric;}

	public string getLengthType() {
		return getUnitType (Types.Length);
	}
	public string getAreaType() {
		return getUnitType (Types.Area);
	}
	public string getWeightType() {
		return getUnitType (Types.Weight);
	}

	public string getUnitType(Types thetype) {
			if (_unit == Units.Metric) {
						return metric [(int)thetype];
				} else if (_unit == Units.Imperial) {
						return imperial [(int)thetype];
				} else {
						throw new Exception ("Tried to get unit type that doesn't exist");
				}
	}

	
	public static float MetersToFeet(float value) {return value * MET2FT;}
	public float getLength(float value) {
				if (_unit == Units.Metric) {
						return value;
				} else if (_unit == Units.Imperial) { 
						return MetersToFeet (value);
				} else {
						throw new Exception ("Unit Length conversion dosen't exist");
				}
		}

	public static float FeetToMeters(float value) {return value / MET2FT;}
	public float setLength(float value) {
		if (_unit == Units.Metric) {
			return value;
		} else if (_unit == Units.Imperial) { 
			return FeetToMeters (value);
		} else {
			throw new Exception ("Unit Length conversion dosen't exist");
		}
	}

	public static float SQMetersToSQFeet(float value) {return value * METSQ2FTSQ;}
	public float getArea(float value) {
			if (_unit == Units.Metric) {
					return value;
			} else if (_unit == Units.Imperial) { 
					return SQMetersToSQFeet (value);
			} else {
					throw new Exception ("Unit Area conversion dosen't exist");
			}
		}

	public static float SQFeetToSQMeters(float value) {return value / METSQ2FTSQ;}
	public float setArea(float value) {
		if (_unit == Units.Metric) {
			return value;
		} else if (_unit == Units.Imperial) { 
			return SQFeetToSQMeters (value);
		} else {
			throw new Exception ("Unit Area conversion dosen't exist");
		}
	}

	public static float KilogramsToPounds(float value) {return value * KG2LB;}
	public float getWeight(float value) {
		if (_unit == Units.Metric) {
			return value;
		} else if (_unit == Units.Imperial) { 
			return KilogramsToPounds (value);
		} else {
			throw new Exception ("Unit Area conversion dosen't exist");
		}
	}
	
	public static float PoundsToKilograms(float value) {return value / KG2LB;}
	public float setWeight(float value) {
		if (_unit == Units.Metric) {
			return value;
		} else if (_unit == Units.Imperial) { 
			return PoundsToKilograms (value);
		} else {
			throw new Exception ("Unit Area conversion dosen't exist");
		}
	}
}
