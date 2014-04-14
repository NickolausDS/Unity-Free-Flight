using UnityEngine;
using System.Collections;
using System;


public class UnitConverter {

	public enum Units{ Metric, Imperial };
	public enum Types { Length, Area, Weight };
	public enum Imperial { Feet, SqFeet, Pounds };
	public enum Metric {Meters, SqMeters, Kilograms };

	private static float METSQ2FTSQ = 10.764f;
	private static float MET2FT = 3.28084f;
	private static float KG2LB = 2.20462f;


	public Units unit = Units.Metric;

	public UnitConverter(Units theunit) {
		unit = theunit;
	}

	public UnitConverter() {unit = Units.Metric;}

//	private static UnitConverter instance;
//	
//	private UnitConverter() {}
//	
//	public static UnitConverter Instance {
//		get {
//			if (instance == null) {
//				instance = new UnitConverter();
//			}
//			return instance;
//		}
//	}
	//I can't think of a use for these, but we'll put them here in case someone needs them
	
	public static float MetersToFeet(float value) {return value * MET2FT;}
	public float getLength(float value) {
				if (unit == Units.Metric) {
						return value;
				} else if (unit == Units.Imperial) { 
						return MetersToFeet (value);
				} else {
						throw new Exception ("Unit Length conversion dosen't exist");
				}
		}

	public static float FeetToMeters(float value) {return value / MET2FT;}
	public float setLength(float value) {
		if (unit == Units.Metric) {
			return value;
		} else if (unit == Units.Imperial) { 
			return FeetToMeters (value);
		} else {
			throw new Exception ("Unit Length conversion dosen't exist");
		}
	}

	public static float SQMetersToSQFeet(float value) {return value * METSQ2FTSQ;}
	public float getArea(float value) {
			if (unit == Units.Metric) {
					return value;
			} else if (unit == Units.Imperial) { 
					return SQMetersToSQFeet (value);
			} else {
					throw new Exception ("Unit Area conversion dosen't exist");
			}
		}

	public static float SQFeetToSQMeters(float value) {return value / METSQ2FTSQ;}
	public float setArea(float value) {
		if (unit == Units.Metric) {
			return value;
		} else if (unit == Units.Imperial) { 
			return SQFeetToSQMeters (value);
		} else {
			throw new Exception ("Unit Area conversion dosen't exist");
		}
	}

	public static float KilogramsToPounds(float value) {return value * KG2LB;}
	public float getWeight(float value) {
		if (unit == Units.Metric) {
			return value;
		} else if (unit == Units.Imperial) { 
			return KilogramsToPounds (value);
		} else {
			throw new Exception ("Unit Area conversion dosen't exist");
		}
	}
	
	public static float PoundsToKilograms(float value) {return value / KG2LB;}
	public float setWeight(float value) {
		if (unit == Units.Metric) {
			return value;
		} else if (unit == Units.Imperial) { 
			return PoundsToKilograms (value);
		} else {
			throw new Exception ("Unit Area conversion dosen't exist");
		}
	}
}
